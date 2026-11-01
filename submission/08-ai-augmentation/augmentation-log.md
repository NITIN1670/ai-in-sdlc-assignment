# AI Augmentation Log

## 1. Objective

This document records how AI tools were used during the development and evaluation of the Invoice Application assignment.

The objective was to use AI to improve engineering productivity while maintaining human oversight, technical correctness, security, and accountability.

AI was used as an engineering assistant for architecture analysis, design decisions, code review, testing, documentation, and evaluation.

Human review and validation remained part of the workflow.

## 2. AI Usage Summary

| Activity | AI assistance | Human contribution |
|---|---|---|
| Architecture analysis | Assisted in understanding the existing application and documenting architecture | Reviewed repository structure and verified implementation details |
| Target architecture | Helped propose architectural improvements and diagrams | Evaluated feasibility and distinguished existing components from proposed components |
| Architecture tests | Suggested architecture boundaries and test implementation | Added tests and executed the test suite |
| ADR preparation | Compared exactly-once submission design alternatives | Reviewed trade-offs and selected the recommended approach |
| Autonomy policy | Helped define autonomy levels and approval controls | Reviewed workflow scope and retained human approval requirements |
| Code-review evaluation | Generated review prompts and helped classify findings | Supplied diffs, reviewed findings, and validated ground truth |
| Risk audit | Identified lethal-trifecta risks and mitigation controls | Checked actual repository configuration and distinguished confirmed capabilities from potential risks |
| Measurement plan | Defined engineering metrics and collection methods | Collected Git history, CI evidence, and test results |
| Documentation | Assisted in drafting and organizing assignment documents | Reviewed and saved the final documents |

## 3. Detailed AI Augmentation Records

### 3.1 Architecture Analysis and Documentation

**AI task:**

Assist with understanding the existing Invoice Application architecture and documenting its current and proposed design.

**AI contribution:**
- Helped organize the architecture description.
- Assisted in preparing Mermaid architecture diagrams.
- Helped identify component responsibilities and dependencies.
- Suggested architectural improvements.

**Human validation:**
- Inspected the actual repository.
- Verified the .NET API, Domain layer, repository implementation, and CI configuration.
- Distinguished implemented components from proposed future components.
- Added and executed architecture boundary tests.

**Outcome:**

Architecture documentation and architecture tests were prepared.

### 3.2 Architecture Test Implementation

**AI task:**

Assist in creating tests to validate architectural boundaries.

**AI contribution:**
- Suggested tests to ensure the Domain layer does not depend on the API layer.
- Suggested checks to prevent direct ASP.NET Core dependencies in the Domain assembly.

**Human validation:**
- Added `DomainArchitectureTests.cs`.
- Executed the .NET test suite.
- Verified the test results.

**Outcome:**

The latest shared test result showed 9 tests passed and 0 failed.

### 3.3 Architecture Decision Record — Exactly-Once Submission

**AI task:**

Evaluate approaches for preventing duplicate payment submissions.

**AI contribution:**
- Compared idempotency keys with deduplication storage.
- Evaluated transactional outbox and worker-based processing.
- Considered optimistic submission and reconciliation.
- Identified failure scenarios, operational costs, and revisit conditions.

**Human validation:**
- Reviewed the design alternatives.
- Considered failure scenarios such as a payment request timing out at 3 AM during month-end processing.
- Evaluated the recommendation against the assignment requirements.
- Retained appropriate caveats around external payment-provider capabilities.

**Outcome:**

ADR-001 was prepared with a recommended design and documented trade-offs.

### 3.4 Autonomy Policy

**AI task:**

Help define a safe autonomy policy for an AI-assisted engineering workflow.

**AI contribution:**
- Proposed autonomy levels from L0 to L4.
- Suggested human approval controls.
- Identified evidence required before increasing autonomy.

**Human validation:**
- Selected the incident-log triage workflow.
- Reviewed the permission boundaries.
- Retained L1 assisted operation as the initial autonomy level.
- Required human approval for higher-impact actions.

**Outcome:**

An autonomy policy was documented with day-one controls and progression criteria.

### 3.5 Agent Evaluation and Prompt Improvement

**AI task:**

Evaluate code-review prompts and assess the agent's ability to detect defects while avoiding false positives.

**AI contribution:**
- Assisted in developing the initial and improved review prompts.
- Reviewed findings from the 10 fixed evaluation cases.
- Helped classify true positives, false positives, and true negatives.
- Assisted in calculating evaluation metrics.
- Suggested stricter evidence requirements to reduce speculative findings.

**Human validation:**
- Supplied the code diffs to the review agent.
- Used fresh sessions for individual test cases.
- Checked findings against the known ground truth.
- Rejected speculative findings.
- Executed three baseline runs and one improved-prompt run.
- Added two custom cases: rejection-reason validation and a behavior-preserving variable rename.

**Observed results:**

| Metric | Baseline v1 | Improved v2 |
|---|---:|---:|
| Reviews | 30 | 10 |
| Recall | 100% | 100% |
| Precision | 91.3% | 100% |
| Accuracy | 93.3% | 100% |
| False alarm rate | 22.2% | 0% |

The two custom cases were also correctly classified.

**Outcome:**

The improved prompt demonstrated better observed precision and fewer false alarms in the evaluation dataset.

The v2 result is limited to one run and requires additional testing to establish repeatability.

### 3.6 Security Risk Audit

**AI task:**

Assess the AI agent setup against the lethal trifecta.

**AI contribution:**
- Identified private-data exposure risks.
- Analyzed untrusted repository and browser content.
- Examined potential external communication risks.
- Suggested least-privilege and human-approval controls.

**Human validation:**
- Inspected the repository configuration.
- Verified the Playwright MCP configuration.
- Distinguished actual configured capabilities from hypothetical integrations.
- Avoided claiming production access or unrestricted external permissions without evidence.

**Outcome:**

A risk audit was prepared with controls, a combined-risk scenario, and revisit triggers.

### 3.7 Engineering Measurement Plan

**AI task:**

Define engineering metrics and prepare a measurement approach.

**AI contribution:**
- Defined lead time for changes.
- Defined change failure rate.
- Defined review turnaround time.
- Suggested GitHub and CI data sources.
- Helped calculate supporting baseline observations.

**Human validation:**
- Inspected Git commit history.
- Verified the GitHub remote.
- Reviewed the CI workflow configuration.
- Inspected the GitHub Actions run.
- Supplied previously collected .NET test results.

**Observed evidence:**
- GitHub Actions CI Run #1 completed successfully in 31 seconds.
- The latest previously shared test execution passed 9 out of 9 tests.
- No merged pull requests were available during measurement.
- Historical PR review and production deployment data were unavailable.

**Outcome:**

A measurement plan was prepared with actual observations and explicit historical-data limitations.

## 4. Human Oversight and Validation

AI-generated content was not automatically accepted as correct.

The following controls were followed:

1. Repository-specific claims were checked against available source code or configuration.
2. Architecture recommendations were reviewed for feasibility.
3. Code-review findings were evaluated against known ground truth.
4. False-positive findings were identified and excluded from defect counts.
5. Tests were executed to validate implementation behavior.
6. Security recommendations were reviewed against actual configured capabilities.
7. Measurement results were based on observed evidence rather than invented numbers.
8. Human approval remained necessary for high-impact engineering actions.

## 5. Examples of AI Limitations Identified

During the evaluation, several limitations were observed:

- The initial code-review prompt sometimes produced speculative findings.
- One baseline review incorrectly reported a documentation-related concern.
- Another baseline review raised an unsupported timestamp-flakiness concern.
- An initial improved-prompt attempt inspected repository files and violated the diff-only evaluation protocol; that result was discarded and the case was rerun correctly.
- A single improved-prompt evaluation run does not establish consistent performance.

These observations demonstrate why AI output must be reviewed and validated rather than accepted without verification.

## 6. Overall Assessment

AI assistance helped accelerate architecture documentation, design comparison, test development, security analysis, code-review evaluation, and measurement planning.

The most valuable contribution was the ability to quickly explore alternatives and generate structured engineering artifacts.

However, engineering decisions, validation, security review, and final acceptance remained human responsibilities.

The assignment demonstrates a human-in-the-loop approach to AI-assisted software development, where AI improves productivity while engineers retain accountability for correctness, quality, and security.

## 7. Conclusion

AI was used as an augmentation tool throughout the software development lifecycle rather than as an autonomous replacement for engineering judgment.

The combination of AI-generated recommendations, repository inspection, automated testing, controlled evaluations, and human review provided a practical approach to responsible AI-assisted development.

**Deliverable 8 — AI Augmentation Logs: Complete.**