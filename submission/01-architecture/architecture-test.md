# Executable Architecture Test

## Architectural rule

The Domain layer must remain independent of the API and ASP.NET Core infrastructure.

## Why this rule matters

- Business rules should not depend on HTTP or web framework concerns.
- The Domain layer should remain independently testable.
- Future infrastructure changes should not require changes to core business rules.
- This supports maintainability and separation of concerns.

## Test implementation

File: `backend/tests/InvoiceApp.Tests/DomainArchitectureTests.cs`

Two rules are enforced:

1. Domain must not reference `InvoiceApp.Api`.
2. Domain must not reference assemblies beginning with `Microsoft.AspNetCore`.

## Execution command

```bash
dotnet test backend/InvoiceApp.sln
```

## Validation

The architecture test is part of the existing xUnit test project and runs as part of the normal solution test command.

## Expected result

- Existing tests: 7
- New architecture tests: 2
- Total: 9
- Expected failures: 0

The test verifies assembly dependencies at runtime. It does not prove all architectural boundaries or runtime behavior.
