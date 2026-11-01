# ADR-001: Reliable and Duplicate-Safe Invoice Payment Submission

**Status:** Proposed  
**Date:** 31 October 2026  
**Decision owners:** Engineering Team

## 1. Context

The Invoice Approval service processes approximately 40,000 invoices per month. Around 60% of invoices arrive during the last three working days, with peak approval activity reaching approximately 80 approvals per minute.

The service integrates with an external payment provider, PayEx.

The current integration scenario has a 30-second timeout. A timeout does not establish whether PayEx processed the payment. Retrying blindly can result in duplicate payments, while not retrying can leave invoices unpaid.

The business requires reliable processing, traceability and seven-year retention of approval actions.

## 2. Decision drivers

- Prevent duplicate payment execution.
- Survive API, worker and infrastructure restarts.
- Handle ambiguous external payment outcomes.
- Preserve an auditable record of every approval and payment attempt.
- Support month-end traffic spikes.
- Keep operational complexity manageable for a six-engineer team.
- Provide clear recovery procedures for on-call engineers.

## 3. Options considered

### Option A: Idempotency key and deduplication table

The API accepts a client-generated idempotency key and stores it with the request result in Azure SQL.

A unique database constraint prevents duplicate processing of the same logical request.

**Advantages**
- Relatively simple to implement.
- Prevents duplicate API submissions.
- Supports safe request retries.
- Low operational overhead.

**Disadvantages**
- Does not independently guarantee reliable asynchronous event delivery.
- Does not solve an ambiguous PayEx outcome.
- Requires a separate mechanism to recover payment processing after failures.

### Option B: Transactional outbox and background worker

The API stores the approval update, audit entry and payment command in a single Azure SQL transaction.

A background worker publishes outbox messages to Azure Service Bus and processes payment commands.

The worker uses a stable PayEx idempotency key and persists provider references and payment outcomes.

**Advantages**
- Approval state and event creation are committed atomically.
- Survives application restarts.
- Supports asynchronous processing and traffic buffering.
- Enables controlled retries and operational monitoring.
- Supports a clear reconciliation workflow.

**Disadvantages**
- More components to operate.
- Requires outbox cleanup and monitoring.
- Requires idempotent consumers because message delivery can be repeated.
- Requires careful handling of unknown payment outcomes.

### Option C: Optimistic submission and nightly reconciliation

The application submits payments optimistically and reconciles discrepancies through a nightly batch process.

**Advantages**
- Lower initial implementation complexity.
- Nightly reconciliation can identify missing or inconsistent outcomes.
- Suitable as a supplementary control.

**Disadvantages**
- Delayed detection of payment discrepancies.
- Poor visibility during month-end processing.
- Does not prevent duplicate payment attempts in real time.
- Recovery may require manual intervention.

## 4. Decision

Select **Option B: Transactional Outbox + Idempotent Payment Worker**, supplemented with an idempotency key and deduplication table.

The recommended processing flow is:

1. The API receives an approval request with an idempotency key.
2. Azure SQL validates and records the request using a unique constraint.
3. One database transaction updates invoice approval state, inserts the audit record and creates the outbox event.
4. The API returns an accepted response after the transaction commits.
5. An outbox publisher sends the event to Azure Service Bus.
6. A payment worker consumes the message.
7. The worker submits the payment to PayEx using a stable provider idempotency key.
8. The worker persists the provider reference and final payment status.
9. Failed or ambiguous outcomes enter a reconciliation workflow.

## 5. Important exactly-once qualification

The architecture provides duplicate-safe business processing, not guaranteed exactly-once network delivery.

Azure Service Bus and the outbox publisher can deliver messages more than once.

The worker must therefore be idempotent.

The PayEx provider must support idempotency keys or an authoritative payment-status lookup to safely resolve ambiguous outcomes.

If PayEx provides neither capability, exactly-once external payment execution cannot be guaranteed by our application alone.

## 6. Failure scenario: 3 AM during month-end

**Scenario:** The payment worker submits a payment to PayEx. PayEx processes the payment successfully, but the response is lost. The worker experiences a 30-second timeout.

The on-call engineer must not immediately resubmit the payment with a new key.

Recovery procedure:

1. Mark the payment as `Unknown` or `PendingReconciliation`.
2. Retain the original idempotency key and correlation ID.
3. Query PayEx using the original key or provider reference.
4. If PayEx confirms success, record the successful payment and close the outbox event.
5. If PayEx confirms failure, retry according to the approved retry policy using the same key.
6. If PayEx cannot establish the outcome, keep the payment in an unresolved state and escalate for manual reconciliation.
7. Record every action in the audit trail.

The engineer should never assume a timeout means the payment failed.

## 7. Operational and cost considerations

| Dimension | Assessment |
|---|---|
| Build cost | Medium to high |
| Run cost | Medium |
| Database cost | Azure SQL storage, transactions and monitoring |
| Messaging cost | Azure Service Bus operations |
| Worker cost | Compute for background processing |
| Monitoring | Queue age, retries, dead-letter count, unknown payment outcomes |
| Operational complexity | Higher than synchronous processing |
| Recovery capability | Strong when reconciliation and provider idempotency are available |

For a six-engineer team, the design should use managed Azure services and avoid unnecessary distributed components.

## 8. Where the agent was wrong or useless

The agent initially treated exactly-once processing as if adding an idempotency key or transactional outbox automatically guaranteed exactly-once payment execution.

That recommendation was incomplete.

An outbox guarantees atomic persistence of business state and event creation, but it does not eliminate duplicate message delivery or resolve an ambiguous external payment outcome.

The external provider's behavior is critical.

The design must explicitly address:
- Provider-side idempotency.
- Stable idempotency keys across retries.
- Payment-status lookup.
- Unknown payment states.
- Reconciliation and manual escalation.

The agent is also not a substitute for verifying PayEx's actual API contract, retention guarantees, timeout behavior and support procedures.

## 9. Revisit triggers

Revisit this decision when:

1. PayEx changes its idempotency or payment-status lookup capabilities.
2. Monthly invoice volume increases significantly.
3. Service Bus operational cost becomes disproportionate.
4. Reconciliation backlog or unknown payment outcomes exceed agreed thresholds.
5. Regulatory or audit retention requirements change.
6. Operational incidents demonstrate that the current retry or recovery strategy is insufficient.

## 10. Consequences

**Positive**
- Durable approval and payment processing.
- Reduced risk of duplicate payments.
- Better recovery from infrastructure failures.
- Improved auditability.
- Controlled asynchronous processing during month-end spikes.

**Negative**
- Additional implementation and operational complexity.
- More monitoring and alerting requirements.
- Dependency on PayEx idempotency or authoritative reconciliation capabilities.
- Need for operational training and documented recovery procedures.
