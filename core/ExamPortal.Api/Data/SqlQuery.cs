namespace ExamPortal.Api.Data;

/// <summary>A SQL statement and its named parameters.</summary>
public sealed class SqlQuery
{
    public SqlQuery(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public Dictionary<string, object?> Parameters { get; } = new();

    public SqlQuery With(string name, object? value)
    {
        Parameters[name] = value;
        return this;
    }
}
