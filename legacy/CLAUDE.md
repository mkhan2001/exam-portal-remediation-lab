# ExamPortal.Legacy (.NET Framework 4.8)

ASP.NET MVC 5 on .NET Framework 4.8, ADO.NET with `System.Data.SqlClient`, SQL Server.

## Commands (Windows only)

- Restore: `nuget restore ExamPortal.Legacy.sln`
- Build: `msbuild ExamPortal.Legacy.sln /p:Configuration=Release`
- Test: `vstest.console.exe ExamPortal.Legacy.Tests\bin\Release\net48\ExamPortal.Legacy.Tests.dll`

This app cannot be built on macOS or Linux. If you are not on Windows, make the change,
say that you could not build it, and rely on the "QA - .NET Framework 4.8 app" check.

## Layout

- `ExamPortal.Legacy.Web/Data/SqlQueries.cs` builds every SQL command. Fix SQL there, not in repositories.
- `ExamPortal.Legacy.Web/Controllers` holds the MVC controllers. Views are Razor files under `Views`.
- `ExamPortal.Legacy.Web/Web.config` holds security-relevant settings.
- `db/schema.sql` creates and seeds the database.

## Rules for this project

- The web project is a classic (non-SDK) project. When you add a `.cs` or `.cshtml` file,
  add it to `ExamPortal.Legacy.Web.csproj` as a `Compile` or `Content` item, or it will not build.
- Packages are in `packages.config`. When you change a package version, update `packages.config`,
  the `HintPath` in the `.csproj`, and the binding redirect in `Web.config` together.
- Views are compiled at runtime with the C# 5 compiler. Do not use `?.`, `$"..."`, or `nameof` in `.cshtml` files.
- Do not use APIs that exist only in .NET Core or later.
- Do not convert the web project to an SDK-style project.
- Tests use MSTest. Test only classes that do not need `HttpContext` or a database.
- If a fix changes how passwords are stored, update the seed data in `db/schema.sql` and
  explain how existing accounts migrate.
