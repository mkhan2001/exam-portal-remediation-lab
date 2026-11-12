using System.Data.Common;

namespace ExamPortal.Api.Data;

/// <summary>Runs <see cref="SqlQuery"/> objects against the database.</summary>
public class QueryExecutor
{
    private readonly Db _db;

    public QueryExecutor(Db db)
    {
        _db = db;
    }

    public List<Dictionary<string, object?>> Read(SqlQuery query)
    {
        using var connection = _db.Open();
        using var command = CreateCommand(connection, query);
        using var reader = command.ExecuteReader();

        var rows = new List<Dictionary<string, object?>>();
        while (reader.Read())
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            rows.Add(row);
        }
        return rows;
    }

    public int Execute(SqlQuery query)
    {
        using var connection = _db.Open();
        using var command = CreateCommand(connection, query);
        return command.ExecuteNonQuery();
    }

    private static DbCommand CreateCommand(DbConnection connection, SqlQuery query)
    {
        var command = connection.CreateCommand();
        command.CommandText = query.Text;
        foreach (var pair in query.Parameters)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = pair.Key;
            parameter.Value = pair.Value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        return command;
    }
}
