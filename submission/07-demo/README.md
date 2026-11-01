# AI in SDLC Workshop – Demo Video

## Overview

This video demonstrates the application of AI across different stages of the Software Development Life Cycle (SDLC), using the Invoice Application as the reference project.

## Demo Video

**Video Link:** (https://nagarro-my.sharepoint.com/:v:/p/megha_agarwal01/IQBrzPkSq3XaR5fcZ2dzLdHOAUMPPZJuXR7vvPmjTMSZy3o?e=s6Qaay)

**Duration:** Approximately 15 minutes

## Topics Covered

1. **Architecture Analysis**
   - As-built architecture based on the existing .NET 8 application.
   - Separation between API and Domain layers.
   - Target architecture and proposed future improvements.
   - Architecture tests validating layer boundaries.

2. **Architecture Decision Record (ADR)**
   - Exactly-once payment submission problem.
   - Evaluation of alternative approaches.
   - Recommended solution and trade-offs.

3. **Autonomy Policy**
   - AI-assisted incident-log triage.
   - Autonomy levels and human approval requirements.

4. **Agent Evaluation**
   - Baseline evaluation across 10 test cases.
   - Comparison of baseline and improved agent results.
   - Custom defect and clean-change scenarios.

5. **Risk Audit**
   - Security risks involving private data, untrusted content, and external communication.
   - Least-privilege access and human oversight.

6. **Measurement Plan**
   - Lead time for changes.
   - Change failure rate.
   - Review turnaround time.
   - Available CI and automated test evidence.

7. **AI Augmentation Log**
   - Documentation of AI-assisted activities throughout the SDLC.
   - Human validation and review of AI-generated outputs.

## Validation Evidence

- Application built using .NET 8.
- Architecture tests added to validate dependency boundaries.
- Latest test execution: 9 passed, 0 failed.
- GitHub Actions CI execution completed successfully.

## Conclusion

The demonstration highlights how AI can support architecture analysis, design decisions, development quality, security reviews, and engineering measurements while maintaining human oversight and validation.