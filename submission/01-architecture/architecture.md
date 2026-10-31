# Invoice Approval Service – Architecture Analysis

## 1. As-built container architecture

The following diagram represents the implementation currently present in the repository, not the proposed production architecture.

```mermaid
flowchart TD
    U["Approver / Admin"]
    API["ASP.NET Core Web API<br/>InvoiceApp.Api<br/>[Program.cs]"]
    AUTH["SessionAuthMiddleware<br/>Bearer token validation<br/>[Auth/SessionAuthMiddleware.cs]"]
    ROLE["RequireRole Action Filter<br/>Approver / Admin<br/>[Auth/RequireRoleAttribute.cs]"]
    CTRL["ApprovalsController<br/>POST approve / reject<br/>[Controllers/ApprovalsController.cs]"]
    SERVICE["ApprovalService<br/>Business lifecycle rules<br/>[Domain/ApprovalService.cs]"]
    INTERFACE["IInvoiceRepository<br/>Repository abstraction<br/>[Domain/IInvoiceRepository.cs]"]
    REPO["InMemoryInvoiceRepository<br/>Process-local List and lock<br/>[InMemoryInvoiceRepository.cs]"]
    DOMAIN["Invoice Domain Model<br/>Status, Amount, DecidedBy,<br/>DecidedAt, DecisionNote<br/>[Domain/Invoice.cs]"]

    U -->|"HTTP POST + Bearer token"| API
    API --> AUTH
    AUTH -->|"Valid session"| ROLE
    ROLE -->|"Authorized role"| CTRL
    CTRL --> SERVICE
    SERVICE --> INTERFACE
    INTERFACE --> REPO
    SERVICE --> DOMAIN
    REPO --> DOMAIN
```

## 2. Approval sequence diagram

```mermaid
sequenceDiagram
    actor Approver
    participant API as ASP.NET Core API
    participant Auth as SessionAuthMiddleware
    participant Role as RequireRole Filter
    participant Controller as ApprovalsController
    participant Service as ApprovalService
    participant Repo as InMemoryInvoiceRepository

    Approver->>API: POST /api/approvals/{id}/approve
    Note over API,Auth: Program.cs registers middleware before MapControllers
    API->>Auth: Validate Bearer token
    Auth->>Auth: SessionStore.Get(token)

    alt Invalid or missing session
        Auth-->>Approver: 401 Unauthorized
    else Valid session
        Auth->>API: Store session in HttpContext
        API->>Role: Execute authorization filter
        Role->>Role: Check Approver or Admin role

        alt Role not permitted
            Role-->>Approver: 403 Forbidden
        else Authorized
            Role->>Controller: Continue action execution
            Controller->>Service: Approve(invoiceId, username)
            Service->>Repo: GetById(invoiceId)
            Repo-->>Service: Invoice
            Service->>Service: EnsurePending()
            Service->>Service: Set Approved, DecidedBy, DecidedAt
            Service->>Repo: Update(invoice)
            Repo-->>Service: Updated invoice
            Service-->>Controller: Approved invoice
            Controller-->>Approver: HTTP response
        end
    end
```

## 3. Inference and evidence register

| Diagram element / finding | Evidence |
|---|---|
| ASP.NET Core API and dependency registration | `backend/src/InvoiceApp.Api/Program.cs` |
| Authentication middleware validates bearer token | `backend/src/InvoiceApp.Api/Auth/SessionAuthMiddleware.cs` |
| Role authorization checks Approver/Admin | `backend/src/InvoiceApp.Api/Auth/RequireRoleAttribute.cs` |
| Approval and rejection endpoints | `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs` |
| Approval business rules and lifecycle | `backend/src/InvoiceApp.Domain/ApprovalService.cs` |
| Repository abstraction | `backend/src/InvoiceApp.Domain/IInvoiceRepository.cs` |
| Process-local in-memory storage | `backend/src/InvoiceApp.Api/InMemoryInvoiceRepository.cs` |
| Invoice decision fields | `backend/src/InvoiceApp.Domain/Invoice.cs` |

### Explicit inferences

1. The application is not currently durable because invoices are held in process memory.
2. Multiple application instances would not automatically share invoice state.
3. The current approval update does not demonstrate an atomic database transaction.
4. The current implementation does not demonstrate a durable audit retention mechanism.
5. There is no implemented PayEx integration or message-based payment workflow.
6. Azure SQL and Azure Service Bus are target-state components described in the assignment scenario, not current application components.

## 4. Target architecture status

The target architecture will be documented separately. Azure SQL, transactional outbox, Service Bus, payment worker, PayEx integration, and reconciliation must be marked as proposed components until implemented and deployed.

