using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using IronReports.Model;

namespace IronReports.Data;

/// <summary>One row of report data. Field names are case-insensitive. Unknown fields return null.</summary>
public interface IReportRow
{
    object? GetValue(string field);
}

/// <summary>Rows plus a field list. Build one from a DataTable, a list of objects, or dictionaries.</summary>
public sealed class ReportDataSource
{
    public ReportDataSource(IReadOnlyList<IReportRow> rows, IReadOnlyList<ReportField> fields)
    {
        Rows = rows;
        Fields = fields;
    }

    public IReadOnlyList<IReportRow> Rows { get; }
    public IReadOnlyList<ReportField> Fields { get; }

    public static ReportDataSource FromDataTable(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var fields = table.Columns.Cast<DataColumn>().Select(c => new ReportField(c.ColumnName, c.DataType.Name)).ToList();
        var rows = table.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted)
            .Select(r => (IReportRow)new DataRowAdapter(r)).ToList();
        return new ReportDataSource(rows, fields);
    }

    public static ReportDataSource FromObjects<T>(IEnumerable<T> items) => FromObjects(items, typeof(T));

    public static ReportDataSource FromObjects(IEnumerable items, Type itemType)
    {
        ArgumentNullException.ThrowIfNull(items);
        var fields = new List<ReportField>();
        CollectFields(itemType, "", fields, depth: 0);
        var rows = items.Cast<object?>().Where(o => o is not null).Select(o => (IReportRow)new ObjectRow(o!)).ToList();
        return new ReportDataSource(rows, fields);
    }

    public static ReportDataSource FromDictionaries(IEnumerable<IReadOnlyDictionary<string, object?>> items)
    {
        var list = items.ToList();
        var fields = new List<ReportField>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var d in list)
        foreach (var kv in d)
            if (seen.Add(kv.Key)) fields.Add(new ReportField(kv.Key, kv.Value?.GetType().Name ?? "Object"));
        var rows = list.Select(d => (IReportRow)new DictionaryRow(d)).ToList();
        return new ReportDataSource(rows, fields);
    }

    private static void CollectFields(Type type, string prefix, List<ReportField> fields, int depth)
    {
        foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.GetIndexParameters().Length > 0) continue;
            var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            var name = prefix + p.Name;
            if (IsSimple(t) || depth >= 2 || typeof(IEnumerable).IsAssignableFrom(t))
                fields.Add(new ReportField(name, t.Name));
            else
                CollectFields(t, name + ".", fields, depth + 1);
        }
    }

    private static bool IsSimple(Type t) =>
        t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) ||
        t == typeof(DateTimeOffset) || t == typeof(TimeSpan) || t == typeof(Guid) || t == typeof(byte[]) ||
        t == typeof(DateOnly) || t == typeof(TimeOnly);

    private sealed class DataRowAdapter : IReportRow
    {
        private readonly DataRow _row;
        public DataRowAdapter(DataRow row) => _row = row;

        public object? GetValue(string field)
        {
            var columns = _row.Table.Columns;
            var index = columns.IndexOf(field);
            if (index < 0) return null;
            var value = _row[index];
            return value is DBNull ? null : value;
        }
    }

    private sealed class DictionaryRow : IReportRow
    {
        private readonly IReadOnlyDictionary<string, object?> _values;
        public DictionaryRow(IReadOnlyDictionary<string, object?> values) => _values = values;

        public object? GetValue(string field)
        {
            if (_values.TryGetValue(field, out var v)) return v is DBNull ? null : v;
            foreach (var kv in _values)
                if (string.Equals(kv.Key, field, StringComparison.OrdinalIgnoreCase)) return kv.Value is DBNull ? null : kv.Value;
            return null;
        }
    }

    /// <summary>Reads public properties by name, including dotted paths such as "Customer.Name".</summary>
    private sealed class ObjectRow : IReportRow
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> Cache = new();
        private readonly object _item;
        public ObjectRow(object item) => _item = item;

        public object? GetValue(string field)
        {
            object? current = _item;
            foreach (var part in field.Split('.'))
            {
                if (current is null) return null;
                var type = current.GetType();
                var prop = Cache.GetOrAdd((type, part), key =>
                    key.Item1.GetProperty(key.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                if (prop is null) return null;
                current = prop.GetValue(current);
            }
            return current;
        }
    }
}
