# Orc.ProjectManagement

Orc.ProjectManagement is a library for managing projects the easy way. It provides a complete project lifecycle management system including loading, saving, refreshing, closing, and activating projects, built on top of the Catel application development platform.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains stable ABI / API. Breaking changes break downstream apps.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.ProjectManagement | `master` |
| Orc.ProjectManagement | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

The repository has protected branches that must be respected.

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Solution Overview

```
src/
  Orc.ProjectManagement/           # Core library
  Orc.ProjectManagement.Example/   # Example application
  Orc.ProjectManagement.Tests/     # Unit tests
```

### Key Namespaces

| Namespace / Directory | Purpose |
|-----------------------|---------|
| `Managers/` | `IProjectManager` / `ProjectManager` — the central facade for all project operations |
| `Models/` | `IProject`, `ProjectBase` — base project model |
| `Serialization/` | `ProjectReaderBase`, `ProjectWriterBase`, serializer selectors |
| `Refreshers/` | `FileProjectRefresher`, `DirectoryProjectRefresher` — watch for external project changes |
| `Validators/` | Validate project locations (file exists, directory exists, etc.) |
| `Upgraders/` | Upgrade projects from older formats |
| `Watchers/` | React to project manager events (e.g. close-before-load, activation history) |
| `Workflow/` | Composable workflow items executed during project operations |
| `Services/` | Supporting services (initialization, configuration, activation history, project state) |

### Directory Guide

| Directory | Editable? | Notes |
|-----------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `deployment/` | No | Deployment / build scripts |
| `src/Orc.ProjectManagement/` | Yes | Core library source |
| `src/Orc.ProjectManagement.Tests/` | Yes | Unit tests |

---

## Writing Code

### Core Concepts

- **`IProjectManager`** is the main entry point. Use it to load, save, refresh, close, and activate projects.
- Projects implement **`IProject`** (typically by extending `ProjectBase`).
- Reading and writing projects is handled by **`ProjectReaderBase`** / **`ProjectWriterBase`**. Register a custom reader/writer pair through the serializer selector.
- File-system watchers (**`FileProjectRefresher`**, **`DirectoryProjectRefresher`**) monitor for external changes and raise `ProjectRefreshRequiredAsync`.
- **Workflow items** (`IProjectWorkflowItem`) allow injecting custom steps into the project load/save/close pipeline.
- **Validators** run before a project is loaded to ensure the location is valid.

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Create a `Facts` class for a feature (e.g. `ProjectManagerFacts`)
3. Nest test classes per method (e.g. `TheLoadMethod`)
4. Use PascalCase for test method names (e.g. `RaisesProjectLoadingEventAsync`)

```csharp
[TestFixture]
public class TheLoadMethod
{
    [TestCase]
    public async Task RaisesProjectLoadingEventAsync()
    {
        var factory = Factory.Create().SetupDefault();
        var projectManager = factory.GetProjectManager();

        var eventRaised = false;
        projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

        await projectManager.LoadAsync("dummyLocation");

        Assert.That(eventRaised, Is.True);
    }
}
```

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|----------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Documentation portal | https://opensource.wildgums.com |
| Catel (base framework) | https://github.com/Catel/Catel |
