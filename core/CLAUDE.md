# ExamPortal.Api (.NET 8)

ASP.NET Core Web API on .NET 8 with cookie authentication. Data access is plain ADO.NET
over SQLite, held in memory and rebuilt on every start, so no database server is needed.

## Commands

- Build: `dotnet build ExamPortal.Api.sln --configuration Release`
- Test: `dotnet test ExamPortal.Api.sln --configuration Release`
- Run: `dotnet run --project ExamPortal.Api`

## Layout

- `ExamPortal.Api/Data/Queries.cs` builds every SQL statement as a `SqlQuery`. Fix SQL there.
- `ExamPortal.Api/Data/Schema.cs` creates and seeds the database, including password hashes.
- `ExamPortal.Api/Controllers` holds the API controllers.
- `ExamPortal.Api/Program.cs` configures authentication, cookies, CORS, and error handling.

## Rules for this project

- Pass values to SQL with `SqlQuery.With("@name", value)`. Column names for sorting must come
  from an allow-list, because they cannot be parameters.
- Use dependency injection, `async`/`await`, and nullable reference types.
- Validate request models with data annotations. `[ApiController]` returns 400 automatically
  when a model is invalid.
- Tests use xUnit. Prefer tests on `Queries` and services that need no web host.
- If a fix changes how passwords are hashed, update the seed hashes in `Schema.cs` and the tests.
- Secrets come from environment variables or user secrets, never from `appsettings.json`.
