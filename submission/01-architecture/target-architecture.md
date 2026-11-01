# Target Architecture – Invoice Approval Service

## 1. Proposed production architecture

The proposed design introduces durable invoice storage, asynchronous payment processing, duplicate protection and reconciliation.

**Important:** All components below are proposed. They are not currently implemented in the repository.

```mermaid
flowchart TD
    USER["Approver / Admin"]
    API["ASP.NET Core 8 API<br/>Azure App Service"]
    AUTH["Authentication and<br/>Role Authorization"]
    SERVICE["Approval Domain Service"]
    DB[("Azure SQL Database")]
    OUTBOX["Transactional Outbox Table"]
    WORKER["Outbox Publisher /<br/>Payment Worker"]
    BUS[["Azure Service Bus"]]
    PAY["PayEx Payment API"]
    AUDIT[("Durable Audit Records")]
    RECON["Reconciliation Worker"]
    MON["Monitoring and Alerting"]

    USER --> API
    API --> AUTH
    AUTH --> SERVICE
    SERVICE -->|"Single SQL transaction"| DB
    DB --- OUTBOX
    DB --- AUDIT
    OUTBOX --> WORKER
    WORKER --> BUS
    BUS --> WORKER
    WORKER -->|"Stable idempotency key"| PAY
    PAY --> WORKER
    WORKER -->|"Persist payment outcome"| DB
    RECON --> DB
    RECON -->|"Query payment status"| PAY
    API --> MON
    WORKER --> MON
    RECON --> MON
```

## 2. Approval and payment sequence

```mermaid
sequenceDiagram
    actor Approver
    participant API as ASP.NET Core API
    participant DB as Azure SQL
    participant Worker as Outbox Worker
    participant SB as Azure Service Bus
    participant PayEx as PayEx

    Approver->>API: Submit approval request with idempotency key
    API->>DB: Begin transaction
    API->>DB: Check idempotency key
    API->>DB: Validate pending invoice
    API->>DB: Update approval state
    API->>DB: Insert audit record
    API->>DB: Insert outbox event
    API->>DB: Commit transaction
    API-->>Approver: Approval accepted

    Worker->>DB: Read unpublished outbox events
    Worker->>SB: Publish payment command
    SB-->>Worker: Message delivered
    Worker->>PayEx: Submit payment with stable idempotency key

    alt Payment succeeds
        PayEx-->>Worker: Success + provider reference
        Worker->>DB: Persist successful payment result
    else Timeout or ambiguous outcome
        PayEx-->>Worker: Timeout / unknown outcome
        Worker->>DB: Mark payment outcome Unknown
        Note over Worker,PayEx: Do not blindly submit a new payment
        Worker->>PayEx: Query status using original reference/key
        PayEx-->>Worker: Payment status
        Worker->>DB: Reconcile and persist final result
    end
```

## 3. Proposed components and responsibilities

| Component | Responsibility | Status |
|---|---|---|
| ASP.NET Core API | Accept invoice approval requests | Existing, target hosting proposed |
| Azure App Service | Host API instances | Proposed |
| Azure SQL | Durable invoice and payment state | Proposed |
| Idempotency records | Prevent duplicate approval submissions | Proposed |
| Transactional outbox | Atomically persist business change and event | Proposed |
| Azure Service Bus | Deliver asynchronous payment commands | Provisioned in scenario; not integrated |
| Payment worker | Process payment commands safely | Proposed |
| PayEx | External payment provider | Scenario only; not integrated |
| Audit records | Retain approval and payment history for seven years | Proposed |
| Reconciliation worker | Resolve unknown payment outcomes | Proposed |
| Monitoring | Track failures, queue age and payment status | Proposed |

## 4. Reliability and security controls

1. Store approval state, audit record and outbox event in one Azure SQL transaction.
2. Use a unique idempotency key with a database uniqueness constraint.
3. Reuse the same provider idempotency key when checking or retrying an ambiguous PayEx request.
4. Treat Service Bus delivery as at-least-once and make consumers idempotent.
5. Never interpret an HTTP timeout as proof that PayEx did not process a payment.
6. Maintain an Unknown or PendingReconciliation state for ambiguous outcomes.
7. Reconcile unknown payment outcomes with PayEx before attempting another payment.
8. Restrict access to payment and audit data using least privilege.
9. Encrypt data in transit and at rest.
10. Apply an approved retention and archival policy for seven-year audit records.

## 5. Important design qualification

The transactional outbox guarantees that the database state change and event creation are committed together. It does not, by itself, guarantee exactly-once external payment execution.

Exactly-once business outcomes require provider-side idempotency or authoritative payment-status lookup, together with local deduplication and reconciliation.

