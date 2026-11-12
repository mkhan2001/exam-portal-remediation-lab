# Practice guide: coding agent, review agent, PR review, and QA

Follow these steps in order. Parts 1 to 5 are one-time setup (about 30 minutes).
Part 6 is the loop you repeat for each finding in `docs/FINDINGS.md`.

What you are building:

```
You describe a finding
   -> coding agent (Claude Code on your machine) writes the fix and a test
   -> pull request
        -> review agent (Claude in GitHub Actions) posts a first-pass review
        -> Automated QA builds both apps and runs the tests
   -> human review and approval
   -> merge -> manual QA confirms the finding is closed
```

---

## Part 1. Put this code in your own GitHub repository

1. Unzip the lab. Open a terminal in the `exam-portal-remediation-lab` folder.
2. Install the GitHub CLI from https://cli.github.com, then sign in:

   ```
   gh auth login
   ```

3. Create a private repository and push the code:

   ```
   git init -b main
   git add .
   git commit -m "Initial import of ExamPortal remediation lab"
   gh repo create exam-portal-remediation-lab --private --source=. --push
   ```

Keep `ANSWER-KEY.md` out of the repository. The agents read everything in the repository,
so committing the answers would defeat the exercise.

## Part 2. Set up the coding agent (Claude Code on your machine)

1. Install Claude Code.

   Windows (PowerShell):

   ```
   irm https://claude.ai/install.ps1 | iex
   ```

   macOS or Linux:

   ```
   curl -fsSL https://claude.ai/install.sh | bash
   ```

2. Check the install and sign in. The first run opens your browser:

   ```
   claude --version
   claude
   ```

3. Confirm it has read the project rules. Inside Claude Code, type:

   ```
   > Summarize the security rules for this repository and the build commands for each app.
   ```

   The answer should mention parameterized SQL, the failing-test rule, and both stacks.
   Those come from `CLAUDE.md` (root), `legacy/CLAUDE.md`, and `core/CLAUDE.md`.

Which machine: the .NET 8 app builds on Windows, macOS, and Linux. The legacy app builds
only on Windows. On a Mac you can still fix legacy code with Claude Code and let the
Windows runner in GitHub Actions do the build and tests.

## Part 3. Set up the review agent (Claude in GitHub Actions)

The workflow files are already in `.github/workflows`. You need to give GitHub two things:
the Claude app and a credential.

1. Install the Claude GitHub App: open https://github.com/apps/claude, choose **Install**,
   and select your `exam-portal-remediation-lab` repository.
2. Add the credential as a repository secret. Pick one:

   - **API key:** create a key at https://platform.claude.com, then run:

     ```
     gh secret set ANTHROPIC_API_KEY
     ```

     Paste the key when asked.

   - **Claude subscription:** run `claude setup-token`, then:

     ```
     gh secret set CLAUDE_CODE_OAUTH_TOKEN
     ```

     Then, in both `claude.yml` and `claude-code-review.yml`, replace the
     `anthropic_api_key` line with the commented `claude_code_oauth_token` line under it.

Alternative: run `/install-github-app` inside Claude Code. It does both steps for you and
opens a pull request that refreshes the two Claude workflow files. Merge that pull request.

What each workflow does:

| File | Agent | Runs when |
|---|---|---|
| `claude-code-review.yml` | Review agent | Every pull request is opened or updated |
| `claude.yml` | Coding agent in GitHub | Someone writes `@claude` in a pull request or issue comment |
| `qa.yml` | No agent, plain build and test | Every pull request and every push to `main` |
| `security.yml` | No agent: SAST, SCA, and DAST scans, report-only at first | Every pull request and every push to `main` |

## Part 4. Check that Automated QA works

1. Open the repository on GitHub and go to the **Actions** tab. The push in Part 1 started
   a run of **Automated QA**.
2. Wait for both jobs: **QA - .NET 8 API** and **QA - .NET Framework 4.8 app**.
3. Both should be green. If the legacy job fails, open the log, copy the error, and ask
   Claude Code to fix it. That is a fair first exercise.

## Part 5. Turn on the gates (branch protection)

In GitHub: **Settings > Branches > Add branch ruleset** (or **Add rule**) for `main`:

1. Require a pull request before merging.
2. Require 1 approval.
3. Require status checks to pass. Add **QA - .NET 8 API** and **QA - .NET Framework 4.8 app**.
4. Block force pushes.

If you are working alone, GitHub will not let you approve your own pull request. Either
add a second account as a collaborator to play the human reviewer, or set approvals to 0
and do the human review step yourself before you merge.

Setup is complete.

---

## Part 6. The remediation loop

Worked example: finding **C-02**, SQL injection in candidate search (.NET 8 app).

### Step 1. Coding agent: understand, then fix

```
git checkout -b fix/c-02-candidate-search-sqli
claude
```

First ask for analysis, not a fix:

```
> Read docs/FINDINGS.md finding C-02. Find the code responsible, trace the full path from
  the endpoint to the database, and list every other place that uses the same pattern.
  Do not change anything yet.
```

Read the answer. Check it against the code. Then:

```
> Fix C-02 following CLAUDE.md. Write the failing test first and show me that it fails.
  Then apply the fix, run the tests, and list the callers that could be affected.
```

Claude shows each edit and asks before applying it. When the tests pass:

```
> Commit this and open a pull request. In the description include the CWE ID, the OWASP
  category, the other instances you found, and how a tester can verify the fix.
```

### Step 2. Review agent: first-pass review

Open the pull request on GitHub. Two things start by themselves:

- **Claude Code Review** posts inline comments on problems it finds, or one summary
  comment if it finds none. It takes a few minutes.
- **Automated QA** builds both apps and runs the tests.

Read each comment and decide: fix it, or reply with why it does not apply. Triage is
the reviewer skill you are practicing.

### Step 3. Fix review findings from the pull request

Write a comment on the pull request, for example:

```
@claude The email search has the same problem. Fix it the same way and add a test for it.
```

Claude pushes a commit to the same branch. The review and QA run again.

### Step 4. Automated QA gate

Wait for both QA jobs to turn green. While either is red, the merge button stays blocked.
For a failure, comment:

```
@claude The legacy QA job failed. Read the log and fix the cause.
```

### Step 5. Human review

Review the pull request yourself, or as your second account. Check four things:

1. Does the fix remove the root cause, or only one symptom?
2. Would the new test have failed before the fix?
3. Did anything change that the finding did not need?
4. Are the upstream and downstream effects listed, and are they right?

Approve and merge.

### Step 6. Manual QA and closure

Run the app (see `README.md`) and repeat the test from the finding. For C-02, search with
a single quote in the term and confirm the request behaves like an ordinary search.
Then record the closure. A GitHub issue per finding works well:

```
gh issue create --title "C-02 SQL injection in candidate search" --body "See docs/FINDINGS.md"
```

Close the issue from the pull request by adding `Closes #<number>` to its description.

---

## Part 7. Security gates: SAST, SCA, and DAST

`security.yml` adds three scans. They start in report-only mode: each job has
`continue-on-error: true`, so a failing scan shows a warning but does not block a merge.
The sample apps are vulnerable on purpose, so enforcing the scans on day one would block
everything. Tightening them step by step is the exercise.

| Job | Type | What it does | Where to read the results |
|---|---|---|---|
| SAST - CodeQL | Static analysis of the source | Scans the C# in both apps without building them | **Security > Code scanning** |
| SCA - vulnerable packages | Dependency scan | Fails if the .NET 8 app uses a package with a known vulnerability | The job log |
| DAST - ZAP baseline scan | Scan of the running app | Starts the .NET 8 API and scans it from outside | The job log and the `zap_scan` artifact |

One-time setup:

1. **Code scanning:** it is free for public repositories. A private repository needs
   GitHub Code Security turned on under **Settings > Advanced Security**. Without it the
   SAST job fails at the upload step. If you cannot enable it, make the repository public
   (the code is a sample) or skip the SAST job.
2. **Dependabot:** under **Settings > Advanced Security**, turn on **Dependabot alerts**
   and **Dependabot security updates**. `.github/dependabot.yml` covers both apps,
   including the legacy `packages.config`, and the workflow actions.

Turning a scan into a gate:

1. Fix finding C-17, then check that the SCA job passes.
2. In `security.yml`, delete the `continue-on-error: true` line from the `sca` job.
3. In branch protection, add **SCA - vulnerable packages** as a required check.
4. Prove the gate works: on a branch, add an old vulnerable package to the .NET 8 project,
   open a pull request, and confirm it cannot merge.
5. Do the same for SAST and DAST once their results are clean, or once you have agreed
   a baseline of accepted findings. This closes finding P-02.

Use the scan output as input for the coding agent:

```
> Here is the SCA job output: <paste>. For each vulnerable package, tell me the CVE, the
  fixed version, and what in this repository would be affected by the upgrade. Change nothing yet.
```

## Part 8. Legacy modernization and dependency analysis

Before migrating a legacy app, list what it depends on. Ask for the analysis first:

```
> Analyze legacy/ for migration to .NET 8. Produce a table of every dependency that blocks
  or complicates the move: NuGet packages, System.Web APIs, configuration, session,
  authentication, serialization, and data access. For each, give the .NET 8 replacement,
  the effort (small, medium, large), and the risk. Change nothing.
```

A good answer covers at least: `System.Web` and MVC 5 to ASP.NET Core MVC, `Global.asax`
and `RouteConfig` to `Program.cs`, `Web.config` to `appsettings.json` and environment
variables, Forms Authentication to cookie authentication, `Session` to the session
middleware, `BinaryFormatter` (removed in modern .NET, so L-11 has to be fixed first),
`System.Data.SqlClient` to `Microsoft.Data.SqlClient`, `HttpPostedFileBase` to `IFormFile`,
and `packages.config` to `PackageReference`.

Then practice the decision you would make as lead, for each part of the app:

- **Migrate as-is:** code that moves without redesign, such as `SqlQueries` and `PasswordHasher`.
- **Re-architect:** code that cannot move safely, such as the preferences cookie and the upload.
- **Fix first, then migrate:** findings that would otherwise be carried into the new app.

Write the result as `docs/MIGRATION-PLAN.md`, then migrate one controller into `core/`
on a branch and put it through the same loop as any other change.

## Part 9. Functional and regression testing

Unit tests prove a fix. Regression tests prove the fix did not break the feature.

1. **Characterization tests before a risky change.** Before fixing L-03, L-11, L-16, C-07,
   or C-18, ask:

   ```
   > Before changing anything, write tests that pin down the current behavior of
     PasswordHasher that callers rely on. They must pass now and must still pass after the fix.
   ```

2. **API-level tests for the .NET 8 app.** Ask Claude to add the
   `Microsoft.AspNetCore.Mvc.Testing` package and write tests with `WebApplicationFactory`
   that call the real endpoints: anonymous access is rejected (C-04, C-21), a Scheduler
   cannot use admin endpoints (C-06), and a user cannot read another user's registration (C-05).
   These are the tests that turn a pen-test finding into a permanent regression check.

3. **A manual QA checklist for what tests cannot reach.** Razor views (L-07, L-08) and
   configuration (L-13) need a short written checklist per finding: steps, expected result,
   and who verified it.

4. **Regression pass before closing a finding.** Confirm the original exploit fails and the
   feature still works: search still finds "O'Brien", login still works for all three
   sample users, and notes still save.

## Suggested order of exercises

| # | Finding | What it teaches |
|---|---|---|
| 1 | C-02 | The basic loop on the app that builds anywhere |
| 2 | L-02 | The same fix on the legacy app, with the Windows QA job |
| 3 | C-04, C-06 | Authentication and role checks, tested through the API |
| 4 | L-09 | Path traversal, where the fix needs a test for the bypass |
| 5 | L-07, L-08 | Fixes in Razor views, which unit tests cannot cover; plan the manual QA |
| 6 | C-07 or L-03 | A fix with downstream impact: existing password hashes stop working |
| 7 | L-13, C-15 | Configuration fixes, where review matters more than tests |
| 8 | L-15, C-17 | Dependency upgrades: package file, references, and binding redirects together |
| 9 | L-11, C-11 | Replacing unsafe deserialization without breaking the feature |
| 10 | C-19 | Command injection, where the existing test must change with the fix |
| 11 | C-20, C-21 | Mass assignment and a fail-open check: small fixes with Critical impact |
| 12 | L-16, C-18 | Replacing weak encryption, with a plan for data already issued |
| 13 | L-17, L-18, L-19 | Three findings in one feature: decide the pull request boundaries |
| 14 | L-20, C-22 | Adding security logging without logging secrets |
| 15 | L-21, P-01, P-02 | Supply chain: scripts, pinned actions, and enforced gates |

## Extra exercises for a team lead

1. **Enforce the gates.** Work through Part 7 until SCA, SAST, and DAST are all required checks.
2. **Triage scanner output.** Compare the CodeQL results with `docs/FINDINGS.md`. Note
   which findings the scanner found, which it missed, and which of its alerts are false
   positives. That comparison is what a lead reports on.
3. **Track the backlog.** Create one issue per finding with severity labels, and a project
   board with columns for Open, In progress, In review, and Closed.
4. **Report.** After five fixes, write a one-page status: findings closed by severity,
   average days from open to merged, what the review agent caught that you had missed,
   and what it got wrong.
5. **Tune the standards.** Each time an agent makes the same mistake twice, add one line
   to the relevant `CLAUDE.md` and see whether the mistake stops.
