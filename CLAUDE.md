# Security remediation standards

This repository contains two ASP.NET applications:

- `legacy/` is ExamPortal.Legacy, an ASP.NET MVC 5 app on .NET Framework 4.8 with SQL Server.
- `core/` is ExamPortal.Api, an ASP.NET Core Web API on .NET 8.

Before building or suggesting an API, check which one the file belongs to.
Build and test commands live in `legacy/CLAUDE.md` and `core/CLAUDE.md`.
The open security findings are listed in `docs/FINDINGS.md`. Fix one finding per pull request.

These rules apply to all code you write and all pull requests you review.
They target OWASP Top 10 risks, with the matching CWE IDs for each rule.

## How to handle a security finding

1. Read the whole code path first: entry point, validation, business logic, data access.
2. Search the solution for the same pattern elsewhere. Report every instance, even outside the diff.
3. Fix the root cause in shared code where possible, not at each call site.
4. Add a unit test that fails before the fix and passes after it.
5. List upstream callers and downstream consumers the change could affect.
6. In the pull request description, name the CWE ID, the OWASP category, and how to verify the fix.

Never weaken a security control to make a test pass or a feature work. Stop and ask.

## Authentication (OWASP: Authentication Failures)

- Every controller and action requires authentication unless explicitly marked anonymous.
  Use `[Authorize]` by default and `[AllowAnonymous]` only with a comment explaining why. (CWE-306, CWE-287)
- Never store or compare passwords in plaintext or with MD5/SHA-1. Use ASP.NET Core Identity's
  `PasswordHasher` on .NET 8, and PBKDF2 (`Rfc2898DeriveBytes`) or ASP.NET Identity on Framework. (CWE-256, CWE-916)
- Enforce account lockout or rate limiting on login, password reset, and MFA endpoints. (CWE-307)
- Login and reset responses must not reveal whether a username exists. (CWE-204)
- No credentials, connection strings, API keys, or tokens in source, config committed to git,
  or test fixtures. Use environment variables or the secret store. (CWE-798)
- JWT: validate issuer, audience, lifetime, and signature. Never accept `alg: none`.
  Never put secrets or personal data in the payload. (CWE-347)
- Password reset tokens must be random, single-use, and expire within 30 minutes. (CWE-640)

## Authorization (OWASP: Broken Access Control)

- Check on the server that the current user may access the specific record, not only the action.
  Never trust an ID from the route, query string, or form without an ownership check. (CWE-639, CWE-862)
- Never rely on hidden fields, disabled buttons, or client-side checks for access control. (CWE-602)

## Input validation and injection (OWASP: Injection)

- Treat all input as untrusted: form fields, query strings, route values, headers, cookies,
  uploaded files, and data read back from the database. (CWE-20)
- Validate on the server with an allow-list: type, length, range, and format.
  Use data annotations or FluentValidation, and check `ModelState.IsValid` before using a model.
- SQL: parameterized queries or stored procedures with parameters only. Never build SQL with
  string concatenation, interpolation, or `string.Format`, including inside stored procedures
  that use dynamic SQL (`sp_executesql` with parameters only). (CWE-89)
- Output: rely on Razor's automatic HTML encoding. Do not use `Html.Raw`, `MvcHtmlString.Create`,
  or `innerHTML` with user data. Encode for the context (HTML, attribute, JavaScript, URL). (CWE-79)
- File paths: never combine user input into a path without `Path.GetFullPath` and a check that
  the result stays inside the allowed folder. (CWE-22)
- Uploads: validate extension and content type against an allow-list, limit size, generate the
  stored file name, and store outside the web root. (CWE-434)
- Redirects: use `Url.IsLocalUrl` before redirecting to a user-supplied URL. (CWE-601)
- Never pass user input to `Process.Start` or a shell. (CWE-78)
- Deserialization: never use `BinaryFormatter`, `NetDataContractSerializer`, `LosFormatter`,
  or Json.NET `TypeNameHandling` other than `None` on untrusted data. (CWE-502)
- XML: set `DtdProcessing = DtdProcessing.Prohibit` and `XmlResolver = null`. (CWE-611)
- Outbound requests: never fetch a URL supplied by the user without an allow-list of hosts. (CWE-918)

## Session management

- Issue a new session identifier after login and after any privilege change.
  On Framework: abandon the session and clear the `ASP.NET_SessionId` cookie before signing in. (CWE-384)
- Session and auth cookies must be `HttpOnly`, `Secure`, and `SameSite=Lax` or `Strict`. (CWE-1004, CWE-614, CWE-1275)
- Set an idle timeout and an absolute timeout. Logout must invalidate the session on the server
  and expire the cookie. (CWE-613)
- Never put session IDs or tokens in URLs. On Framework, `sessionState` must use `cookieless="UseCookies"`. (CWE-598)
- All state-changing requests (POST, PUT, DELETE) need anti-forgery protection:
  `[ValidateAntiForgeryToken]` on Framework, `[AutoValidateAntiforgeryToken]` globally on .NET 8.
  Never change state in a GET. (CWE-352)
- Do not store passwords, full card numbers, or other sensitive data in session or cookies. (CWE-315)

## Configuration checks (OWASP: Security Misconfiguration)

Framework `web.config`:
- `<httpCookies httpOnlyCookies="true" requireSSL="true" />`
- `<customErrors mode="On" />` or `RemoteOnly`; `<compilation debug="false" />`
- `<forms requireSSL="true" slidingExpiration="true" timeout="..."/>` with a deliberate timeout
- Request validation stays enabled. Do not add `validateRequest="false"` or `[ValidateInput(false)]`.

.NET 8:
- `UseHttpsRedirection` and `UseHsts` in production
- Cookie options: `HttpOnly = true`, `SecurePolicy = Always`, `SameSite = Lax` or `Strict`
- Developer exception page only in Development

## Cryptography (OWASP: Cryptographic Failures)

- No DES, 3DES, RC4, MD5, or SHA-1 for protecting data. Use AES-GCM, or AES-CBC with an HMAC. (CWE-327)
- Never use ECB mode, and never reuse a fixed IV. (CWE-327, CWE-329)
- No encryption keys in source. Load keys from the secret store. (CWE-321)
- Security tokens and random values come from `RandomNumberGenerator`, never `System.Random`. (CWE-330)
- On .NET 8, prefer the Data Protection API (`IDataProtector`) over hand-written encryption.

## Design rules (OWASP: Insecure Design, Software or Data Integrity Failures)

- Bind requests to a model that contains only the fields the caller may set. Never bind
  directly to a role, owner, or price field. (CWE-915)
- Never build a shell command from user input. Call the program directly with an argument
  list, and validate each argument against an allow-list. (CWE-78)

## Failing safely (OWASP: Mishandling of Exceptional Conditions)

- Security checks fail closed. If an authorization or validation step throws, deny the request. (CWE-636)
- No empty `catch` blocks. Log the exception, then handle it or rethrow. (CWE-390)

## Errors and logging

- Never return stack traces, SQL errors, or internal paths to the client. (CWE-209)
- Log authentication failures, access-control failures, and input-validation failures with
  user ID, source IP, and timestamp. (CWE-778)
- Never log passwords, tokens, session IDs, or full personal data. (CWE-532)

## Dependencies and supply chain (OWASP: Software Supply Chain Failures)

- Do not add or upgrade a NuGet package without stating why in the pull request.
- Flag any package with a known vulnerability and name the CVE.
- Scripts and styles load from this application, or from a CDN over HTTPS with an
  `integrity` attribute. (CWE-829, CWE-353)
- In workflows, pin third-party actions to a full commit SHA.

## When reviewing a pull request

- Report violations of the rules above as findings, with the file, line, and CWE ID.
- Rate severity: critical (exploitable without login), high (exploitable by any logged-in user),
  medium (needs specific conditions), low (hardening).
- Check that each security fix has a test, and that the same pattern is not left elsewhere.
- Do not report style issues in a security fix pull request.
