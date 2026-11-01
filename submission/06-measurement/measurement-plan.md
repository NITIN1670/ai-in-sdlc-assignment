# Engineering Measurement Plan

## 1. Objective

The objective of this measurement plan is to establish a baseline for evaluating software delivery performance and quality in the Invoice Application project.

The plan focuses on three engineering metrics:

1. Lead Time for Changes.
2. Change Failure Rate.
3. Review Turnaround Time.

These metrics help evaluate delivery speed, production stability, and code-review efficiency.

The measurements will use Git history, GitHub pull requests, GitHub Actions, test results, and deployment/incident records.

## 2. Measurement Scope

The project uses:
- GitHub for source control.
- GitHub Actions for CI automation.
- .NET 8 for the backend application.
- xUnit for unit testing.
- Playwright for end-to-end testing.

The CI workflow is configured to run on:
- Pushes to the `main` branch.
- Pull requests targeting the `main` branch.

The workflow performs:
1. Repository checkout.
2. .NET 8 SDK setup.
3. Release configuration build.
4. Release configuration unit tests.

## 3. Engineering Metrics

### 3.1 Lead Time for Changes

**Definition:**

Lead Time for Changes measures the time taken for a code change to move from development to integration.

For this project, the operational measurement will use the time between pull-request creation and successful merge into the main branch.

**Formula:**

Lead Time = PR Merge Timestamp − PR Creation Timestamp

The metric will be reported in hours or days.

**Data source:**

GitHub Pull Requests:
https://github.com/NITIN1670/ai-in-sdlc-assignment/pulls

**Exact collection query:**

GitHub Pull Requests filter:

`is:pr is:merged`

For each merged PR, collect:
- PR number.
- Created timestamp.
- Merged timestamp.
- PR title.
- Associated commit.

**Measurement process:**

1. Retrieve merged PRs from GitHub.
2. Record PR creation and merge timestamps.
3. Calculate the duration for each PR.
4. Calculate the median and average lead time.
5. Track changes in lead time over time.

**Baseline measurement:**

No merged pull requests were available in the repository when the baseline was collected.

Therefore, historical PR-based lead time cannot currently be calculated.

**Future target:**

Establish the first measurable baseline after the first pull request is merged. Track the median lead time monthly.

### 3.2 Change Failure Rate

**Definition:**

Change Failure Rate measures the percentage of production deployments that result in a failure requiring remediation.

Examples include:
- Production incidents.
- Rollbacks.
- Emergency hotfixes.
- Failed production releases requiring corrective action.

**Formula:**

Change Failure Rate =

(Failed Production Deployments / Total Production Deployments) × 100

**Data sources:**

1. GitHub Actions:
   https://github.com/NITIN1670/ai-in-sdlc-assignment/actions

2. Production deployment records.

3. Incident and rollback records.

**Exact collection approach:**

For GitHub Actions, inspect workflow runs and identify:
- Workflow name.
- Commit SHA.
- Branch.
- Execution result.
- Run timestamp.

For production deployments, maintain a deployment record containing:
- Deployment ID.
- Deployment timestamp.
- Commit SHA.
- Deployment outcome.
- Whether an incident, rollback, or hotfix occurred.

**Measurement process:**

1. Count production deployments during the measurement period.
2. Identify deployments resulting in production incidents, rollbacks, or hotfixes.
3. Divide failed deployments by total production deployments.
4. Report the result as a percentage.

**Baseline measurement:**

The repository currently has a CI workflow but no verified production deployment history available for this measurement.

One successful GitHub Actions CI run was observed. It must not be counted as a successful production deployment.

Therefore, Change Failure Rate is currently not available.

**Future target:**

Start collecting deployment and incident records for every production release. Establish the initial failure-rate baseline after sufficient deployment history is available.

### 3.3 Review Turnaround Time

**Definition:**

Review Turnaround Time measures how long a pull request waits before receiving its first human review.

**Formula:**

Review Turnaround Time =

First Human Review Timestamp − PR Creation Timestamp

The metric will be reported in hours.

**Data source:**

GitHub Pull Requests and review history:

https://github.com/NITIN1670/ai-in-sdlc-assignment/pulls

**Exact collection query:**

GitHub Pull Requests filter:

`is:pr is:merged`

For each PR, collect:
- PR creation timestamp.
- First human review timestamp.
- Reviewer.
- Review outcome.
- PR merge timestamp.

**Measurement process:**

1. Retrieve pull-request history.
2. Identify the first human review submitted for each PR.
3. Calculate the time between PR creation and first review.
4. Calculate average and median review turnaround.
5. Track the metric monthly.

**Baseline measurement:**

No merged pull requests or associated review history were available when the baseline was collected.

Therefore, historical review turnaround cannot currently be calculated.

**Future target:**

Collect review timestamps for every pull request and establish the initial baseline after the first reviews are completed.

## 4. Actual Baseline Measurements

The following measurements were collected from the repository and CI environment.

### 4.1 GitHub Actions CI Execution

**Source:** GitHub Actions workflow history.

Repository:
https://github.com/NITIN1670/ai-in-sdlc-assignment/actions

Observed execution:

| Attribute | Observed value |
|---|---|
| Workflow | CI |
| Run | CI #1 |
| Commit | `0616158` |
| Branch | main |
| Trigger | Push |
| Execution date | 14 October 2026 |
| Duration | 31 seconds |
| Result | Success |

**Observation:**

The CI workflow completed successfully in 31 seconds.

The workflow executed the configured Release build and unit-test steps.

This provides an actual baseline observation for CI execution duration.

### 4.2 Unit Test Execution

**Source:** Previous `dotnet test` execution in the project.

Command:

`dotnet test backend/InvoiceApp.sln --configuration Release`

Observed latest result after adding architecture tests:

| Attribute | Result |
|---|---:|
| Total tests | 9 |
| Passed | 9 |
| Failed | 0 |
| Pass rate | 100% |

**Calculation:**

Test Pass Rate = (9 / 9) × 100 = 100%

**Observation:**

All nine tests passed, including the two architecture tests added to validate that the Domain layer does not depend on the API layer or ASP.NET Core.

The test execution duration was not recorded and is therefore not reported.

## 5. Consolidated Baseline Summary

| Measurement | Baseline | Evidence status |
|---|---:|---|
| CI execution duration | 31 seconds | Measured |
| Unit test pass rate | 100% (9/9) | Measured |
| Lead time for changes | Not available | No merged PR history |
| Change failure rate | Not available | No verified production deployment history |
| Review turnaround | Not available | No PR review history |

The two measured engineering observations are CI execution duration and unit-test pass rate.

They provide supporting evidence of build and test quality but are not substitutes for the three primary delivery metrics.

## 6. Measurement Collection Frequency

| Metric | Collection frequency | Reporting |
|---|---|---|
| Lead Time for Changes | Every merged PR | Monthly |
| Change Failure Rate | Every production deployment | Monthly |
| Review Turnaround Time | Every PR review | Monthly |
| CI execution duration | Every CI run | Weekly |
| Unit test pass rate | Every CI run | Weekly |

## 7. Data Collection and Reporting

The following process will be used to maintain the measurement baseline:

1. Collect GitHub PR timestamps and review history.
2. Capture GitHub Actions execution results and duration.
3. Record unit-test results from CI.
4. Maintain a deployment and incident history.
5. Calculate the three primary engineering metrics.
6. Compare monthly results against the established baseline.
7. Identify trends and improvement opportunities.

A monthly measurement report should include:
- Number of merged pull requests.
- Median lead time.
- Median review turnaround.
- Total production deployments.
- Failed production deployments.
- Change failure rate.
- CI execution duration.
- Unit-test pass rate.

## 8. Relationship to AI-Assisted Development

These metrics will help evaluate whether AI-assisted development improves engineering productivity without reducing software quality.

Expected areas of improvement:

**Lead Time for Changes**
- Faster implementation through AI-assisted coding.
- Reduced time spent on repetitive development tasks.
- Faster test generation and defect identification.

**Review Turnaround Time**
- AI-assisted initial code review.
- Earlier detection of security and business logic issues.
- Reduced manual effort spent identifying common defects.

**Change Failure Rate**
- Better defect detection before merge.
- Improved test coverage.
- More consistent validation before deployment.

AI-generated findings and code changes must still be validated by human engineers.

The measurement plan does not assume that AI automatically improves these metrics. Actual changes will be evaluated using collected evidence.

## 9. Limitations and Measurement Gaps

The following limitations were identified during baseline collection:

1. The repository has no merged PR history available for historical lead-time measurement.
2. No PR review timestamps are available for review-turnaround measurement.
3. No verified production deployment history is available for change-failure-rate measurement.
4. The observed CI duration is based on one successful workflow run.
5. The unit-test result is a snapshot of the latest previously shared test execution.
6. The two supporting measurements are not sufficient to establish long-term performance trends.

These gaps will be addressed as additional pull requests, reviews, CI runs, and production deployments are recorded.

## 10. Conclusion

The measurement plan establishes a practical framework for evaluating engineering delivery speed, review efficiency, and software stability.

Two actual baseline observations were captured:

- GitHub Actions CI completed successfully in 31 seconds.
- The latest unit-test execution passed all nine tests with zero failures.

The three primary engineering metrics have been defined with their formulas, data sources, collection methods, and reporting frequency.

Historical values for lead time, change failure rate, and review turnaround are currently unavailable because the repository does not yet have the required merged PR, review, and production deployment history.

These metrics will be collected as the project progresses.

**Deliverable 6 — Engineering Measurement Plan: Complete.**