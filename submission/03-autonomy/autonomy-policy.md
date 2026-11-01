# AI Agent Autonomy Policy – Month-End Incident Log Triage

## 1. Purpose

Define the permitted autonomy of an AI agent assisting engineers with month-end invoice approval incidents.

The agent helps the on-call engineer analyze logs, identify likely failure patterns, correlate events and prepare an incident summary.

The agent does not independently change production systems or make payment decisions.

## 2. Workflow scope

**Selected workflow:** Month-end incident log triage.

The agent may:
- Read approved application logs and monitoring outputs.
- Identify error patterns and recurring exceptions.
- Correlate timestamps, invoice IDs and correlation IDs.
- Compare incidents with recent deployment changes.
- Suggest possible root causes.
- Prepare a summary with evidence and source references.
- Recommend investigation steps.

The agent may not:
- Execute production commands.
- Modify application code or infrastructure.
- Restart services or workers.
- Change invoice or payment status.
- Submit or retry payments.
- Access unrestricted customer or payment credentials.
- Send external communications without human approval.

## 3. Autonomy levels

| Level | Description | Permission |
|---|---|---|
| L0 – Manual | Engineer performs all investigation | No agent action |
| L1 – Assisted | Agent summarizes supplied logs | Read-only, human-directed |
| L2 – Supervised | Agent investigates approved log sources and proposes findings | Human reviews every finding |
| L3 – Conditional | Agent performs predefined investigation and creates an internal incident draft | Restricted automation with approval gates |
| L4 – Autonomous | Agent independently executes incident response actions | Not permitted for this workflow |

## 4. Day-one autonomy level

**Approved starting level: L1 – Assisted.**

The agent can summarize logs supplied by the on-call engineer and identify possible error patterns.

All findings must include supporting log references.

The engineer remains responsible for validating conclusions and deciding the next action.

## 5. Human approval requirements

Human approval is mandatory before:

1. Restarting an application or worker.
2. Changing configuration.
3. Releasing or retrying payment messages.
4. Modifying invoice or payment state.
5. Deploying a code or infrastructure change.
6. Communicating incident conclusions externally.

The agent cannot bypass these controls.

## 6. Security controls

- Apply least-privilege access to approved log sources.
- Mask customer information, credentials and payment-sensitive data.
- Treat log contents and external payloads as untrusted input.
- Prevent log content from overriding agent instructions.
- Disable production write permissions.
- Restrict external communication tools.
- Record prompts, evidence, findings and human approvals.
- Retain audit records according to company policy.

## 7. Evidence required to increase autonomy

Progression from L1 to L2 requires:

- At least 30 representative incident investigations.
- No critical security or privacy violations.
- Evidence-backed findings.
- Acceptable false-positive and missed-issue rates.
- Review and approval from the engineering lead.
- Documented failure and escalation procedures.

Progression to L3 requires a separate security and operational review, successful evaluation results and explicit approval from the service owner.

## 8. Accountability

| Responsibility | Owner |
|---|---|
| Agent configuration | Engineering team |
| Incident validation | On-call engineer |
| Autonomy approval | Engineering lead |
| Security approval | Security team |
| Production response | Authorized human operator |

## 9. Monitoring and rollback

Review agent findings and errors during every on-call rotation.

Immediately suspend agent access if:
- Sensitive data is exposed.
- Instructions from untrusted logs influence agent behavior.
- Findings cannot be traced to evidence.
- The agent attempts unauthorized actions.
- Incident summaries repeatedly produce misleading conclusions.

## 10. Review frequency

Review this policy quarterly and after any significant production incident, security event or agent capability change.

**Final decision:** Start with L1 assisted log triage. Increase autonomy only through measured evidence and explicit human approval.
