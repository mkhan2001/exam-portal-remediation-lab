using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace ExamPortal.Api.Data;

/// <summary>
/// Owns the SQLite database. The database lives in memory and is rebuilt on
/// every start, so the sample needs no database server.
/// </summary>
public class Db
{
    private readonly string _connectionString;
    private DbConnection? _keepAlive;

    public Db(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ExamPortal")
            ?? "Data Source=examportal;Mode=Memory;Cache=Shared";
    }

    public DbConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Initialize()
    {
        // An in-memory database disappears when its last connection closes,
        // so hold one open for the lifetime of the app.
        _keepAlive = Open();

        using var command = _keepAlive.CreateCommand();
        command.CommandText = Schema.CreateAndSeed;
        command.ExecuteNonQuery();
    }
}
