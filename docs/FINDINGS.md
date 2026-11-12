# Security assessment findings

Simulated findings for the two ExamPortal applications and their pipeline, as they would
arrive from a penetration test, a DAST scan, a SAST scan, and an SCA scan.
The report tells you what was observed, not where the code is or how to fix it.
Finding that out is the exercise.

- **Severity:** Critical (exploitable without login), High (exploitable by any logged-in
  user), Medium (needs specific conditions), Low (hardening).
- **OWASP:** category in the OWASP Top 10:2025. The mapping is by CWE and is approximate;
  check it against the edition your client reports on.
- **Found by:** DAST (seen in the running app), SAST (seen in the source), SCA (dependency
  scan), Pen test (manual testing), Review (configuration or pipeline review).

OWASP Top 10:2025 categories: A01 Broken Access Control, A02 Security Misconfiguration,
A03 Software Supply Chain Failures, A04 Cryptographic Failures, A05 Injection,
A06 Insecure Design, A07 Authentication Failures, A08 Software or Data Integrity Failures,
A09 Security Logging and Alerting Failures, A10 Mishandling of Exceptional Conditions.

## ExamPortal.Legacy (.NET Framework 4.8, `legacy/`)

| ID | Finding | Severity | OWASP | CWE | Found by | Where observed |
|---|---|---|---|---|---|---|
| L-01 | SQL injection in the login form: a crafted username changes the query | Critical | A05 | CWE-89 | DAST | `POST /Account/Login` |
| L-02 | SQL injection in candidate search by last name | High | A05 | CWE-89 | DAST, SAST | `GET /Candidates?search=` |
| L-03 | Passwords stored with a fast, unsalted hash | High | A04 | CWE-916 | SAST | Users table, login |
| L-04 | Session identifier is not renewed at login, and logout leaves the server session alive | Medium | A07 | CWE-384, CWE-613 | Pen test | Login and logout |
| L-05 | State-changing requests have no anti-forgery token, and a candidate can be deleted with a GET link | High | A01 | CWE-352 | DAST | Notes update, delete, preferences, import, login |
| L-06 | Open redirect after login through `returnUrl` | Medium | A01 | CWE-601 | DAST | `/Account/Login?returnUrl=` |
| L-07 | Reflected cross-site scripting in the search results heading | High | A05 | CWE-79 | DAST | `GET /Candidates?search=` |
| L-08 | Stored cross-site scripting in candidate notes | High | A05 | CWE-79 | Pen test | Candidate details page |
| L-09 | Report download reads any file on the server, without login, and the error message reveals the server path | Critical | A01 | CWE-22, CWE-306, CWE-209 | DAST | `GET /Reports/Download?file=` |
| L-10 | Any logged-in user can view, edit, and delete candidates owned by other users | High | A01 | CWE-639, CWE-862 | Pen test | Candidate details, notes update, delete |
| L-11 | Preferences cookie is deserialized with an unsafe formatter | Critical | A08 | CWE-502 | SAST | `ep_prefs` cookie on `/Candidates` |
| L-12 | Login error messages reveal whether a username exists, and there is no lockout | Low | A07 | CWE-204, CWE-307 | Pen test | `POST /Account/Login` |
| L-13 | Insecure configuration: debug build, detailed errors shown to users, cookies without `HttpOnly` and `Secure`, session ID allowed in the URL, 8-hour session and 30-day login lifetime | Medium | A02 | CWE-1004, CWE-614, CWE-598, CWE-209, CWE-613 | DAST, Review | Application configuration |
| L-14 | Database credentials for a production server stored in the repository | High | A07 | CWE-798 | SAST | Application configuration |
| L-15 | Dependency with a known vulnerability: Newtonsoft.Json 9.0.1 | Medium | A03 | CWE-1395 | SCA | `packages.config` |
| L-16 | Report access codes are encrypted with DES using a key and IV stored in the source | High | A04 | CWE-327, CWE-321, CWE-329 | SAST | Candidate details page |
| L-17 | Roster import resolves external entities in uploaded XML (XXE) | High | A02 | CWE-611 | SAST, Pen test | `POST /Import/Upload` |
| L-18 | Roster upload accepts any file type, keeps the client's file name, and saves under the web root; the name can also point outside the upload folder | High | A06 | CWE-434, CWE-22 | Pen test | `POST /Import/Upload` |
| L-19 | Import errors are silently discarded, so a failed import looks the same as an empty one | Low | A10 | CWE-390 | SAST | `POST /Import/Upload` |
| L-20 | No security logging: logins, failed logins, deletions, and imports leave no record | Medium | A09 | CWE-778 | Review | Whole application |
| L-21 | Script loaded from a third-party CDN over HTTP with no integrity check; the library is jQuery 1.12.4, which has known vulnerabilities | Medium | A03, A08 | CWE-829, CWE-353, CWE-1395 | DAST, SCA | Every page |

## ExamPortal.Api (.NET 8, `core/`)

| ID | Finding | Severity | OWASP | CWE | Found by | Where observed |
|---|---|---|---|---|---|---|
| C-01 | SQL injection in the login request | Critical | A05 | CWE-89 | DAST | `POST /api/auth/login` |
| C-02 | SQL injection in candidate search, reachable without login | Critical | A05 | CWE-89 | DAST, SAST | `GET /api/candidates?search=` |
| C-03 | SQL injection through the sort column | High | A05 | CWE-89 | SAST | `GET /api/registrations/by-center?sort=` |
| C-04 | Candidate endpoints require no authentication | Critical | A07 | CWE-306 | DAST | `/api/candidates` |
| C-05 | Any logged-in user can read any registration by ID | High | A01 | CWE-639 | Pen test | `GET /api/registrations/{id}` |
| C-06 | Admin endpoints check for a login but not for the Admin role | High | A01 | CWE-862 | Pen test | `/api/admin/*` |
| C-07 | Passwords stored with a fast, unsalted hash | High | A04 | CWE-916 | SAST | Users table, login |
| C-08 | Passwords written to the application log | High | A09 | CWE-532 | SAST | `POST /api/auth/login` |
| C-09 | Login and reset responses reveal whether a username exists, and there is no lockout | Low | A07 | CWE-204, CWE-307 | Pen test | Login, reset request |
| C-10 | Password reset design: the token is a guessable 6-digit number, is returned in the response, never expires, and can be reused | Critical | A06, A07 | CWE-640, CWE-330 | Pen test | `/api/auth/reset-request`, `/api/auth/reset-confirm` |
| C-11 | Import endpoint deserializes JSON with type names enabled | High | A08 | CWE-502 | SAST | `POST /api/admin/import` |
| C-12 | Roster fetch requests any URL the caller supplies (SSRF) | High | A01 | CWE-918 | Pen test | `GET /api/admin/fetch-roster?url=` |
| C-13 | Registration requests are not validated: empty exam codes, negative seats, and free-text dates are accepted | Medium | A05 | CWE-20 | DAST | `POST /api/registrations` |
| C-14 | Login cookie lacks `HttpOnly` and `Secure`, uses `SameSite=None`, lasts 30 days, and logout is a GET | Medium | A02 | CWE-1004, CWE-614, CWE-1275, CWE-613 | DAST | Cookie configuration, logout |
| C-15 | Insecure configuration: developer error page in production, any origin allowed with credentials, no HTTPS redirect or HSTS | Medium | A02, A10 | CWE-209, CWE-942, CWE-319 | DAST, Review | Application startup |
| C-16 | Admin API key stored in the repository | High | A07 | CWE-798 | SAST | Application configuration |
| C-17 | Dependency with a known vulnerability: Newtonsoft.Json 12.0.3 | Medium | A03 | CWE-1395 | SCA | Project file |
| C-18 | Report link tokens are encrypted with AES in ECB mode using a key stored in the source, with no integrity check, so tokens can be read and forged | High | A04 | CWE-327, CWE-321 | SAST | `GET /api/registrations/{id}/report-link` |
| C-19 | Operating system command injection through the archive name | Critical | A05 | CWE-78 | SAST, Pen test | `POST /api/admin/archive?name=` |
| C-20 | Mass assignment: a user can change their own role through the profile update | Critical | A08 | CWE-915 | Pen test | `PUT /api/auth/profile` |
| C-21 | Access check fails open: updating candidate notes succeeds for a caller who is not logged in | Critical | A10 | CWE-636, CWE-306 | Pen test | `PUT /api/candidates/{id}/notes` |
| C-22 | Security events are not logged: failed logins, deletions, role changes, and denied requests | Medium | A09 | CWE-778 | Review | Whole application |

## Pipeline (`.github/`)

| ID | Finding | Severity | OWASP | CWE | Found by | Where observed |
|---|---|---|---|---|---|---|
| P-01 | Third-party actions are referenced by version tag, which the action's owner can move, instead of by commit SHA | Medium | A03 | CWE-829 | Review | Workflow files |
| P-02 | SAST, SCA, and DAST run in report-only mode, so a pull request with new vulnerabilities can still merge | Medium | A03, A06 | CWE-1357 | Review | `security.yml`, branch protection |

## Coverage by OWASP Top 10:2025 category

| Category | Legacy | .NET 8 |
|---|---|---|
| A01 Broken Access Control | L-05, L-06, L-09, L-10 | C-05, C-06, C-12 |
| A02 Security Misconfiguration | L-13, L-17 | C-14, C-15 |
| A03 Software Supply Chain Failures | L-15, L-21 | C-17 (and P-01, P-02) |
| A04 Cryptographic Failures | L-03, L-16 | C-07, C-18 |
| A05 Injection | L-01, L-02, L-07, L-08 | C-01, C-02, C-03, C-13, C-19 |
| A06 Insecure Design | L-18 | C-10 |
| A07 Authentication Failures | L-04, L-12, L-14 | C-04, C-09, C-16 |
| A08 Software or Data Integrity Failures | L-11, L-21 | C-11, C-20 |
| A09 Security Logging and Alerting Failures | L-20 | C-08, C-22 |
| A10 Mishandling of Exceptional Conditions | L-19 | C-15, C-21 |

## Closure criteria

A finding is closed when all of these are true:

1. The root cause is fixed, and the same pattern is fixed everywhere else it appears.
2. A unit test exists that failed before the fix and passes after it, where a unit test can cover it.
3. Both Automated QA checks pass.
4. A human reviewer has approved the pull request.
5. The original test from this report no longer succeeds against the deployed build.
