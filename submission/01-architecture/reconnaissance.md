# Repository Reconnaissance – Invoice Approval Service

## 1. Existing application architecture

The repository contains a demo Invoice Approval application built using ASP.NET Core 8.

The solution has the following main components:

- **API:** ASP.NET Core Web API project (`InvoiceApp.Api`).
- **Domain:** Business logic and invoice lifecycle management (`InvoiceApp.Domain`).
- **Persistence:** `InMemoryInvoiceRepository`, registered as a singleton.
- **Authentication:** Session-based authentication middleware.
- **Authorization:** Role-based authorization using `RequireRole`.
- **Testing:** xUnit unit tests and Playwright end-to-end tests.

## 2. Current invoice approval flow

1. An approver sends an HTTP POST request to the approval endpoint.
2. Session authentication middleware validates the user's session.
3. The controller requires the Approver or Admin role.
4. `ApprovalsController` calls `ApprovalService.Approve`.
5. `ApprovalService` retrieves the invoice using `IInvoiceRepository`.
6. The service validates that the invoice is in PendingApproval status.
7. The service changes the status to Approved.
8. The approver identity and decision timestamp are recorded.
9. The repository is updated and the API returns the result.

## 3. Important source files

| File | Responsibility |
|---|---|
| `backend/src/InvoiceApp.Api/Program.cs` | Application setup and dependency injection |
| `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs` | Approval and rejection endpoints |
| `backend/src/InvoiceApp.Api/InMemoryInvoiceRepository.cs` | In-memory invoice persistence |
| `backend/src/InvoiceApp.Domain/ApprovalService.cs` | Invoice approval business rules |
| `backend/src/InvoiceApp.Domain/IInvoiceRepository.cs` | Repository abstraction |
| `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs` | Approval service unit tests |
| `.mcp.json` | Playwright MCP configuration |
| `AGENTS.md` | Repository development instructions |

## 4. Existing implementation limitations

- Invoice data is stored in process memory and is not durable.
- Restarting the application can result in loss of invoice data.
- In-memory storage is not suitable for a multi-instance production deployment.
- There is no Azure SQL integration in the current code.
- Azure Service Bus is not integrated into the application.
- PayEx payment integration is not implemented.
- There is no transactional outbox implementation.
- There is no demonstrated durable seven-year audit retention mechanism.

## 5. Scenario versus implementation

The assignment describes a planned production scenario involving Azure SQL, Azure Service Bus, PayEx, high-volume invoice processing and SOC 2 requirements.

These should be treated as target-state requirements, not existing implementation components.

## 6. Validation commands

Run unit tests:

```bash
dotnet test backend/InvoiceApp.sln
```

Run the API:

```bash
dotnet run --project backend/src/InvoiceApp.Api
```

Run end-to-end tests:

```bash
cd e2e
npm install
npx playwright install chromium
npm test
```

## 7. Initial architectural risks

1. Data loss due to in-memory persistence.
2. Concurrent approval consistency across application instances.
3. Duplicate payment submission following an ambiguous PayEx timeout.
4. Lack of durable audit history.
5. No reliable asynchronous payment processing mechanism.
6. No demonstrated reconciliation process for uncertain payment outcomes.

---

## 8. Baseline test execution

Environment: GitHub Codespaces
Framework: .NET 8
Command: dotnet test backend/InvoiceApp.sln

Result:
- Total tests: 7
- Passed: 7
- Failed: 0
- Skipped: 0
- Build succeeded in 18.5 seconds.

This is the baseline before architecture or application code changes.

**Evidence note:** Findings are based on the repository source files listed above. Target-state components are explicitly distinguished from implemented components.