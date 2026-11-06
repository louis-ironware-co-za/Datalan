using System.Globalization;
using System.Text;

namespace IronReports.Expressions;

internal enum TokenKind
{
    Number,
    String,
    Field,      // [Name] or [Customer.Name]
    Parameter,  // ?Name
    Identifier,
    Operator,
    LParen,
    RParen,
    Comma,
    End
}

internal readonly record struct Token(TokenKind Kind, string Text, int Position, object? Value = null);

internal static class Lexer
{
    private static readonly string[] Operators =
    {
        "==", "!=", "<>", "<=", ">=", "&&", "||", "+", "-", "*", "/", "%", "<", ">", "=", "!"
    };

    public static List<Token> Tokenize(string source)
    {
        var tokens = new List<Token>();
        var i = 0;
        while (i < source.Length)
        {
            var c = source[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }

            var start = i;
            if (char.IsDigit(c) || (c == '.' && i + 1 < source.Length && char.IsDigit(source[i + 1])))
            {
                while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.')) i++;
                if (i < source.Length && (source[i] == 'e' || source[i] == 'E'))
                {
                    var j = i + 1;
                    if (j < source.Length && (source[j] == '+' || source[j] == '-')) j++;
                    if (j < source.Length && char.IsDigit(source[j]))
                    {
                        i = j;
                        while (i < source.Length && char.IsDigit(source[i])) i++;
                    }
                }
                var text = source[start..i];
                object value;
                if (text.Contains('e') || text.Contains('E'))
                    value = double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
                else if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
                    value = d;
                else
                    throw new ExpressionException($"Invalid number '{text}'", start);
                // Suffix: 'd' forces double, 'm' forces decimal (optional, for clarity).
                if (i < source.Length && (source[i] == 'd' || source[i] == 'D') && !IsIdentChar(Peek(source, i + 1)))
                {
                    value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    i++;
                }
                else if (i < source.Length && (source[i] == 'm' || source[i] == 'M') && !IsIdentChar(Peek(source, i + 1)))
                {
                    i++;
                }
                tokens.Add(new Token(TokenKind.Number, text, start, value));
                continue;
            }

            if (c == '\'' || c == '"')
            {
                var quote = c;
                var sb = new StringBuilder();
                i++;
                var closed = false;
                while (i < source.Length)
                {
                    if (source[i] == quote)
                    {
                        if (i + 1 < source.Length && source[i + 1] == quote) { sb.Append(quote); i += 2; continue; }
                        i++;
                        closed = true;
                        break;
                    }
                    sb.Append(source[i]);
                    i++;
                }
                if (!closed) throw new ExpressionException("Unterminated string", start);
                tokens.Add(new Token(TokenKind.String, sb.ToString(), start, sb.ToString()));
                continue;
            }

            if (c == '[')
            {
                var end = source.IndexOf(']', i + 1);
                if (end < 0) throw new ExpressionException("Missing ']' after field name", start);
                var name = source[(i + 1)..end].Trim();
                if (name.Length == 0) throw new ExpressionException("Empty field name", start);
                tokens.Add(new Token(TokenKind.Field, name, start, name));
                i = end + 1;
                continue;
            }

            if (c == '?')
            {
                i++;
                while (i < source.Length && IsIdentChar(source[i])) i++;
                var name = source[(start + 1)..i];
                if (name.Length == 0) throw new ExpressionException("Missing parameter name after '?'", start);
                tokens.Add(new Token(TokenKind.Parameter, name, start, name));
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                while (i < source.Length && IsIdentChar(source[i])) i++;
                tokens.Add(new Token(TokenKind.Identifier, source[start..i], start));
                continue;
            }

            if (c == '(') { tokens.Add(new Token(TokenKind.LParen, "(", i++)); continue; }
            if (c == ')') { tokens.Add(new Token(TokenKind.RParen, ")", i++)); continue; }
            if (c == ',') { tokens.Add(new Token(TokenKind.Comma, ",", i++)); continue; }

            var op = Operators.FirstOrDefault(o => string.CompareOrdinal(source, i, o, 0, o.Length) == 0);
            if (op is null) throw new ExpressionException($"Unexpected character '{c}'", i);
            tokens.Add(new Token(TokenKind.Operator, op, i));
            i += op.Length;
        }
        tokens.Add(new Token(TokenKind.End, "", source.Length));
        return tokens;
    }

    private static char Peek(string s, int i) => i < s.Length ? s[i] : '\0';

    private static bool IsIdentChar(char c) => char.IsLetterOrDigit(c) || c == '_';
}
