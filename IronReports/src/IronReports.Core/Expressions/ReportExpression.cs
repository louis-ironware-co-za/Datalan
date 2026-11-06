using System.Collections.Concurrent;

namespace IronReports.Expressions;

internal delegate object? Evaluator(EvalContext ctx);

/// <summary>
/// A parsed, compiled expression.
/// Grammar (lowest precedence first): or, and, not, comparison, + -, * / %, unary -, primary.
/// Primary: number, 'text', "text", true, false, null, [Field], ?Parameter, Function(args), (expression).
/// Keywords and function names are case-insensitive.
/// </summary>
public sealed class ReportExpression
{
    private static readonly ConcurrentDictionary<string, ReportExpression> Cache = new();
    private readonly Evaluator _evaluator;

    private ReportExpression(string source, Evaluator evaluator, bool usesPageInfo, IReadOnlyCollection<string> fields)
    {
        Source = source;
        _evaluator = evaluator;
        UsesPageInfo = usesPageInfo;
        Fields = fields;
    }

    public string Source { get; }

    /// <summary>True when the expression calls PageNumber() or PageCount(), so it must be evaluated after pagination.</summary>
    public bool UsesPageInfo { get; }

    /// <summary>Field names referenced by the expression.</summary>
    public IReadOnlyCollection<string> Fields { get; }

    public object? Evaluate(EvalContext ctx) => _evaluator(ctx);

    /// <summary>Parses and compiles an expression. Results are cached by source text.</summary>
    public static ReportExpression Compile(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return Cache.GetOrAdd(source, static s =>
        {
            var parser = new Parser(s);
            var evaluator = parser.ParseAll();
            return new ReportExpression(s, evaluator, parser.UsesPageInfo, parser.Fields);
        });
    }

    /// <summary>Returns null when the expression is valid, otherwise the error message.</summary>
    public static string? Validate(string source)
    {
        try
        {
            Compile(source);
            return null;
        }
        catch (ExpressionException ex)
        {
            return ex.Message;
        }
    }

    public static object? Evaluate(string source, EvalContext ctx) => Compile(source).Evaluate(ctx);

    public override string ToString() => Source;

    private sealed class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;

        public Parser(string source)
        {
            _tokens = Lexer.Tokenize(source);
            if (_tokens.Count == 1) throw new ExpressionException("Expression is empty");
        }

        public bool UsesPageInfo { get; private set; }
        public HashSet<string> Fields { get; } = new(StringComparer.OrdinalIgnoreCase);

        private Token Current => _tokens[_pos];

        public Evaluator ParseAll()
        {
            var e = ParseOr();
            if (Current.Kind != TokenKind.End) throw new ExpressionException($"Unexpected '{Current.Text}'", Current.Position);
            return e;
        }

        private bool IsKeyword(string word) =>
            Current.Kind == TokenKind.Identifier && string.Equals(Current.Text, word, StringComparison.OrdinalIgnoreCase);

        private bool IsOperator(string op) => Current.Kind == TokenKind.Operator && Current.Text == op;

        private Evaluator ParseOr()
        {
            var left = ParseAnd();
            while (IsOperator("||") || IsKeyword("or"))
            {
                _pos++;
                var l = left;
                var r = ParseAnd();
                left = ctx => Operators.ToBool(l(ctx)) || Operators.ToBool(r(ctx));
            }
            return left;
        }

        private Evaluator ParseAnd()
        {
            var left = ParseNot();
            while (IsOperator("&&") || IsKeyword("and"))
            {
                _pos++;
                var l = left;
                var r = ParseNot();
                left = ctx => Operators.ToBool(l(ctx)) && Operators.ToBool(r(ctx));
            }
            return left;
        }

        private Evaluator ParseNot()
        {
            if (IsOperator("!") || IsKeyword("not"))
            {
                _pos++;
                var inner = ParseNot();
                return ctx => !Operators.ToBool(inner(ctx));
            }
            return ParseComparison();
        }

        private Evaluator ParseComparison()
        {
            var left = ParseAdditive();
            if (Current.Kind != TokenKind.Operator) return left;
            var op = Current.Text;
            if (op is not ("=" or "==" or "!=" or "<>" or "<" or "<=" or ">" or ">=")) return left;
            _pos++;
            var right = ParseAdditive();
            var l = left;
            return op switch
            {
                "=" or "==" => ctx => Operators.AreEqual(l(ctx), right(ctx), ctx.Culture),
                "!=" or "<>" => ctx => !Operators.AreEqual(l(ctx), right(ctx), ctx.Culture),
                "<" => ctx => Relational(l(ctx), right(ctx), ctx, c => c < 0),
                "<=" => ctx => Relational(l(ctx), right(ctx), ctx, c => c <= 0),
                ">" => ctx => Relational(l(ctx), right(ctx), ctx, c => c > 0),
                _ => ctx => Relational(l(ctx), right(ctx), ctx, c => c >= 0)
            };
        }

        private static object Relational(object? a, object? b, EvalContext ctx, Func<int, bool> test) =>
            a is not null && b is not null && test(Operators.Compare(a, b, ctx.Culture));

        private Evaluator ParseAdditive()
        {
            var left = ParseMultiplicative();
            while (IsOperator("+") || IsOperator("-"))
            {
                var op = Current.Text;
                _pos++;
                var l = left;
                var r = ParseMultiplicative();
                left = op == "+"
                    ? ctx => Operators.Add(l(ctx), r(ctx), ctx.Culture)
                    : ctx => Operators.Subtract(l(ctx), r(ctx), ctx.Culture);
            }
            return left;
        }

        private Evaluator ParseMultiplicative()
        {
            var left = ParseUnary();
            while (IsOperator("*") || IsOperator("/") || IsOperator("%"))
            {
                var op = Current.Text;
                _pos++;
                var l = left;
                var r = ParseUnary();
                left = op switch
                {
                    "*" => ctx => Operators.Multiply(l(ctx), r(ctx), ctx.Culture),
                    "/" => ctx => Operators.Divide(l(ctx), r(ctx), ctx.Culture),
                    _ => ctx => Operators.Modulo(l(ctx), r(ctx), ctx.Culture)
                };
            }
            return left;
        }

        private Evaluator ParseUnary()
        {
            if (IsOperator("-"))
            {
                _pos++;
                var inner = ParseUnary();
                return ctx => Operators.Negate(inner(ctx), ctx.Culture);
            }
            if (IsOperator("+"))
            {
                _pos++;
                return ParseUnary();
            }
            return ParsePrimary();
        }

        private Evaluator ParsePrimary()
        {
            var t = Current;
            switch (t.Kind)
            {
                case TokenKind.Number:
                case TokenKind.String:
                {
                    _pos++;
                    var value = t.Value;
                    return _ => value;
                }
                case TokenKind.Field:
                {
                    _pos++;
                    var name = t.Text;
                    Fields.Add(name);
                    return ctx => ctx.Row?.GetValue(name);
                }
                case TokenKind.Parameter:
                {
                    _pos++;
                    var name = t.Text;
                    return ctx => ctx.Parameters.TryGetValue(name, out var v)
                        ? v
                        : throw new ExpressionException($"Unknown parameter '?{name}'");
                }
                case TokenKind.LParen:
                {
                    _pos++;
                    var inner = ParseOr();
                    Expect(TokenKind.RParen, "')'");
                    return inner;
                }
                case TokenKind.Identifier:
                    return ParseIdentifier();
                default:
                    throw new ExpressionException(t.Kind == TokenKind.End ? "Unexpected end of expression" : $"Unexpected '{t.Text}'", t.Position);
            }
        }

        private Evaluator ParseIdentifier()
        {
            var t = Current;
            _pos++;
            var word = t.Text;
            if (Current.Kind != TokenKind.LParen)
            {
                if (word.Equals("true", StringComparison.OrdinalIgnoreCase)) return _ => true;
                if (word.Equals("false", StringComparison.OrdinalIgnoreCase)) return _ => false;
                if (word.Equals("null", StringComparison.OrdinalIgnoreCase)) return _ => null;
                throw new ExpressionException($"Unknown name '{word}'. Fields are written in brackets, for example [{word}]", t.Position);
            }

            _pos++; // (
            var args = new List<Evaluator>();
            if (Current.Kind != TokenKind.RParen)
            {
                args.Add(ParseOr());
                while (Current.Kind == TokenKind.Comma)
                {
                    _pos++;
                    args.Add(ParseOr());
                }
            }
            Expect(TokenKind.RParen, "')'");

            if (word.Equals("PageNumber", StringComparison.OrdinalIgnoreCase) ||
                word.Equals("PageCount", StringComparison.OrdinalIgnoreCase))
                UsesPageInfo = true;

            return Functions.Bind(word, args.ToArray(), t.Position);
        }

        private void Expect(TokenKind kind, string what)
        {
            if (Current.Kind != kind) throw new ExpressionException($"Expected {what}", Current.Position);
            _pos++;
        }
    }
}
