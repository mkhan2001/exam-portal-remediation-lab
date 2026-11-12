# ExamPortal remediation lab

Two small sample applications with security vulnerabilities planted on purpose, plus the
GitHub workflows and `CLAUDE.md` files for practicing AI-assisted remediation:
a coding agent writes the fix, a review agent reviews the pull request, automated QA gates
the merge, and a human approves.

> **Warning: intentionally vulnerable code.** Run it only on your own machine.
> Never deploy it, and never reuse this code in a real application.

## What is in here

| Path | What it is |
|---|---|
| `legacy/` | ExamPortal.Legacy: ASP.NET MVC 5 on .NET Framework 4.8, ADO.NET, SQL Server |
| `core/` | ExamPortal.Api: ASP.NET Core Web API on .NET 8, cookie authentication, in-memory SQLite |
| `docs/FINDINGS.md` | Simulated assessment report: 45 findings covering every OWASP Top 10:2025 category, with severity, CWE IDs, and the scan type that found each |
| `docs/PRACTICE.md` | Step-by-step setup and the remediation loop. Start here. |
| `CLAUDE.md` | Security remediation standards that both agents follow |
| `legacy/CLAUDE.md`, `core/CLAUDE.md` | Build commands and rules for each application |
| `.github/workflows/` | Review agent, `@claude` assistant, Automated QA, and SAST, SCA, and DAST scans |
| `.github/dependabot.yml` | Dependency alerts and update pull requests for both apps and the workflows |

## How the lab maps to a remediation role

| Requirement | Where you practice it |
|---|---|
| C#, .NET Framework/.NET, ASP.NET, SQL Server | `legacy/` (MVC 5, .NET Framework 4.8, SQL Server) and `core/` (ASP.NET Core, .NET 8) |
| OWASP Top 10 | Every 2025 category appears in both apps: see the coverage table in `docs/FINDINGS.md` |
| SAST, DAST, and SCA remediation | Each finding is labeled with the scan type that found it; `security.yml` runs all three |
| Secure SDLC and CI/CD security gates | Branch protection, required QA checks, and report-only scans you turn into gates (`docs/PRACTICE.md`, Parts 5 and 7) |
| Legacy modernization and dependency analysis | `docs/PRACTICE.md`, Part 8; findings L-15, L-21, C-17 |
| AI-assisted remediation | Coding agent and review agent, driven by the three `CLAUDE.md` files |
| Functional and regression testing | Existing unit tests that must keep passing, plus `docs/PRACTICE.md`, Part 9 |
| Leading remediation across teams | Team lead exercises at the end of `docs/PRACTICE.md`: gates, backlog, reporting, standards |
| Full context through production closure | The closure criteria in `docs/FINDINGS.md` and the six-step loop in Part 6 |

## Run the .NET 8 API (Windows, macOS, or Linux)

Needs the .NET 8 SDK.

```
cd core
dotnet test
dotnet run --project ExamPortal.Api -- --urls http://localhost:5080
```

Try it:

```
curl "http://localhost:5080/api/candidates?search=Br"
curl -c cookies.txt -H "Content-Type: application/json" \
     -d '{"username":"alice","password":"Passw0rd!"}' http://localhost:5080/api/auth/login
curl -b cookies.txt http://localhost:5080/api/registrations/1
```

Endpoints:

| Method | Path | Purpose |
|---|---|---|
| POST | `/api/auth/login` | Log in |
| GET | `/api/auth/logout` | Log out |
| POST | `/api/auth/reset-request`, `/api/auth/reset-confirm` | Password reset |
| PUT | `/api/auth/profile` | Update your profile |
| GET | `/api/candidates?search=`, `/api/candidates/by-email?email=`, `/api/candidates/{id}` | Candidates |
| PUT | `/api/candidates/{id}/notes` | Update candidate notes |
| GET | `/api/registrations/{id}`, `/api/registrations/by-center?testCenter=&sort=`, `/api/registrations/{id}/report-link` | Registrations |
| POST | `/api/registrations` | Create a registration |
| DELETE | `/api/admin/candidates/{id}` | Delete a candidate |
| POST | `/api/admin/import` | Import a JSON payload |
| POST | `/api/admin/archive?name=` | Build an export archive |
| GET | `/api/admin/fetch-roster?url=` | Fetch a roster from a URL |

## Run the legacy app (Windows only)

Needs Visual Studio 2019 or 2022 with the "ASP.NET and web development" workload, and
SQL Server LocalDB (installed with Visual Studio).

1. Create the database:

   ```
   sqllocaldb start MSSQLLocalDB
   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE ExamPortalLegacy"
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ExamPortalLegacy -i legacy\db\schema.sql
   ```

2. Open `legacy\ExamPortal.Legacy.sln` in Visual Studio. It restores the NuGet packages.
3. Set `ExamPortal.Legacy.Web` as the startup project and press F5.

From a Developer Command Prompt you can also build and test without the IDE:

```
nuget restore legacy\ExamPortal.Legacy.sln
msbuild legacy\ExamPortal.Legacy.sln /p:Configuration=Release
vstest.console.exe legacy\ExamPortal.Legacy.Tests\bin\Release\net48\ExamPortal.Legacy.Tests.dll
```

## Sample logins (both apps)

| Username | Password | Role |
|---|---|---|
| alice | Passw0rd! | Scheduler |
| bob | Winter2024 | Scheduler |
| admin | admin123 | Admin |

## Next step

Open `docs/PRACTICE.md` and follow it from Part 1.
