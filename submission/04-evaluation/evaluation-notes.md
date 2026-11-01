# AI Code Review Agent — Evaluation Notes

## 1. Objective

The objective is to evaluate an AI code-review agent's ability to:

- Detect real defects introduced by code changes.
- Identify security, business logic, concurrency, and reliability issues.
- Avoid false-positive findings on clean code changes.
- Provide evidence-based findings with realistic failure scenarios.
- Improve review quality through controlled prompt refinement.

The evaluation uses 10 predefined code-review cases, an initial prompt (v1), an improved prompt (v2), and two additional custom cases.

## 2. Evaluation Dataset

The original dataset contains 10 fixed code-review cases.

| Case | Expected result | Description |
|---|---|---|
| case-01 | Defect | Null handling in bulk approval |
| case-02 | Defect | SQL injection through interpolated parameters |
| case-03 | Defect | Incorrect monetary rounding behavior |
| case-04 | Clean | Consistent repository field and parameter rename |
| case-05 | Defect | Missing Admin authorization on DELETE endpoint |
| case-06 | Clean | Test-only changes |
| case-07 | Defect | Sensitive invoice and session data logged |
| case-08 | Defect | Thread-safety and non-atomic dictionary insertion |
| case-09 | Clean | Documentation and logging configuration changes |
| case-10 | Defect | Catch-all exception handler returns HTTP 200 |

**Ground truth:**
- Total original cases: 10
- Defect cases: 7
- Clean cases: 3

### Additional custom cases

Two additional cases were created to test the agent on business-rule validation and behavior-preserving refactoring.

| Custom case | Expected result | Description |
|---|---|---|
| custom-01 | Defect | Removal of mandatory invoice rejection-reason validation |
| custom-02 | Clean | Consistent local-variable rename without behavior change |

## 3. Evaluation Methodology

### 3.1 Baseline evaluation — Prompt v1

The initial prompt instructed the agent to review the supplied diff and report:

- File and approximate line number.
- Description of the defect.
- What could go wrong.
- Confidence level.
- An explicit statement when no defect was found.

The baseline evaluation was conducted over three independent runs.

Each run used:
- The same 10 evaluation cases.
- Auto model selection.
- A fresh agent session for each case.
- The supplied code diff as the review input.

### 3.2 Improved evaluation — Prompt v2

The prompt was improved to reduce speculative findings and enforce evidence-based code review.

The revised prompt introduced these constraints:

1. Review only the supplied code diff.
2. Do not inspect other repository files or documentation.
3. Do not execute commands or use external tools.
4. Report only demonstrable defects introduced by the change.
5. Do not invent business requirements or assume undocumented behavior.
6. Avoid hypothetical timing and concurrency concerns without a concrete failure mechanism.
7. Include a realistic failure scenario and impact for every finding.
8. Respond exactly "No defect found." when no demonstrable defect exists.

The improved prompt was evaluated against the same 10 cases using the same Auto model selection.

### 3.3 Controlled experiment

| Variable | Baseline v1 | Improved v2 |
|---|---|---|
| Model | Auto | Auto |
| Prompt | v1 | v2 |
| Dataset | 10 fixed cases | Same 10 cases |
| Number of runs | 3 | 1 |
| Review context | Supplied diff | Supplied diff |
| Session approach | Fresh session per case | Fresh session per case |

The intended independent variable was the review prompt. The model and original evaluation dataset remained unchanged.

The two additional custom cases were evaluated separately using v2.

## 4. Baseline Results — Prompt v1

### 4.1 Run 1

| Case | Agent result | Evaluation |
|---|---|---|
| case-01 | Caught | True positive |
| case-02 | Caught | True positive |
| case-03 | Caught | True positive |
| case-04 | Clean | True negative |
| case-05 | Caught | True positive |
| case-06 | Clean | True negative |
| case-07 | Caught | True positive |
| case-08 | Caught | True positive |
| case-09 | False alarm | False positive |
| case-10 | Caught | True positive |

**Run 1 metrics:**

- TP: 7
- FN: 0
- TN: 2
- FP: 1
- Recall: 100%
- Precision: 87.5%
- Accuracy: 90%
- False alarm rate: 33.3%

### 4.2 Run 2

| Case | Agent result | Evaluation |
|---|---|---|
| case-01 | Caught | True positive |
| case-02 | Caught | True positive |
| case-03 | Caught | True positive |
| case-04 | Clean | True negative |
| case-05 | Caught | True positive |
| case-06 | Clean | True negative |
| case-07 | Caught | True positive |
| case-08 | Caught | True positive |
| case-09 | Clean | True negative |
| case-10 | Caught | True positive |

**Run 2 metrics:**

- TP: 7
- FN: 0
- TN: 3
- FP: 0
- Recall: 100%
- Precision: 100%
- Accuracy: 100%
- False alarm rate: 0%

### 4.3 Run 3

| Case | Agent result | Evaluation |
|---|---|---|
| case-01 | Caught | True positive |
| case-02 | Caught | True positive |
| case-03 | Caught | True positive |
| case-04 | Clean | True negative |
| case-05 | Caught | True positive |
| case-06 | False alarm | False positive |
| case-07 | Caught | True positive |
| case-08 | Caught | True positive |
| case-09 | Clean | True negative |
| case-10 | Caught | True positive |

**Run 3 metrics:**

- TP: 7
- FN: 0
- TN: 2
- FP: 1
- Recall: 100%
- Precision: 87.5%
- Accuracy: 90%
- False alarm rate: 33.3%

### 4.4 Combined baseline results

Across three runs, the agent completed 30 reviews.

| Metric | Result |
|---|---:|
| Total reviews | 30 |
| True positives | 21 |
| False negatives | 0 |
| True negatives | 7 |
| False positives | 2 |
| Recall | 100% |
| Precision | 91.3% |
| Accuracy | 93.3% |
| Specificity | 77.8% |
| False alarm rate | 22.2% |

**Baseline observations:**

- The agent detected all seven known defects in all three runs.
- Two false-positive findings occurred across nine clean-case opportunities.
- In case-09, the agent incorrectly considered README reset instructions misleading.
- In case-06, the agent raised a speculative concern about timestamp assertion reliability.
- The agent demonstrated strong defect detection but sometimes reported concerns that were not supported by sufficient evidence.

## 5. Improved Results — Prompt v2

The improved prompt was evaluated against all 10 original cases in a fourth run.

### 5.1 Case-level results

| Case | Expected | Actual | Evaluation |
|---|---|---|---|
| case-01 | Defect | Caught | TP |
| case-02 | Defect | Caught | TP |
| case-03 | Defect | Caught | TP |
| case-04 | Clean | No defect found | TN |
| case-05 | Defect | Caught | TP |
| case-06 | Clean | No defect found | TN |
| case-07 | Defect | Caught | TP |
| case-08 | Defect | Caught | TP |
| case-09 | Clean | No defect found | TN |
| case-10 | Defect | Caught | TP |

### 5.2 v2 metrics

| Metric | Result |
|---|---:|
| Total reviews | 10 |
| True positives | 7 |
| False negatives | 0 |
| True negatives | 3 |
| False positives | 0 |
| Recall | 100% |
| Precision | 100% |
| Accuracy | 100% |
| Specificity | 100% |
| False alarm rate | 0% |

**Observations:**

- All seven known defects were detected.
- All three clean cases were correctly classified.
- No false-positive findings were recorded.
- The agent provided concrete failure scenarios and confidence levels for detected defects.
- The stricter prompt helped prevent unsupported assumptions and speculative findings.

## 6. Custom Evaluation Results — Prompt v2

Two additional custom cases were evaluated using the improved prompt, Auto model selection, and fresh agent sessions.

### 6.1 Custom-01 — Rejection reason validation

**Expected:** Defect.

The custom diff removed the existing validation that requires a meaningful rejection reason before an invoice can be rejected.

**Actual:** Caught correctly.

The agent identified that removing the validation allows null, empty, or whitespace rejection reasons to be accepted.

**Impact identified:**
- Rejected invoices may lack an actionable explanation.
- Auditability is weakened.
- Domain data may become inconsistent.
- Investigating or defending rejection decisions becomes difficult.

**Evaluation:** True positive.

### 6.2 Custom-02 — Local variable rename

**Expected:** Clean.

The custom diff consistently renamed a local variable without changing the business logic or execution behavior.

**Actual:** No defect found.

The agent correctly classified the change as a behavior-preserving refactoring.

**Evaluation:** True negative.

### 6.3 Custom case summary

| Custom case | Expected | Actual | Evaluation |
|---|---|---|---|
| custom-01 | Defect | Caught | TP |
| custom-02 | Clean | No defect found | TN |

**Custom evaluation metrics:**

| Metric | Result |
|---|---:|
| Reviews | 2 |
| True positives | 1 |
| False negatives | 0 |
| True negatives | 1 |
| False positives | 0 |
| Recall | 100% |
| Precision | 100% |
| Accuracy | 100% |
| False alarm rate | 0% |

These results are descriptive of the two custom cases only and should not be treated as statistically conclusive.

## 7. Baseline vs Improved Prompt Comparison

The comparison below uses only the original 10 fixed cases to ensure equivalent evaluation datasets.

| Metric | Baseline v1 | Improved v2 |
|---|---:|---:|
| Number of reviews | 30 | 10 |
| True positives | 21 | 7 |
| False negatives | 0 | 0 |
| True negatives | 7 | 3 |
| False positives | 2 | 0 |
| Recall | 100% | 100% |
| Precision | 91.3% | 100% |
| Accuracy | 93.3% | 100% |
| False alarm rate | 22.2% | 0% |

### Key findings

The improved prompt showed better precision and fewer false alarms in the observed evaluation.

The main improvement came from:

- Requiring findings to be directly supported by changed code.
- Requiring realistic failure scenarios.
- Excluding undocumented assumptions.
- Preventing speculative concurrency and timing concerns.
- Explicitly instructing the agent to report no defect when the evidence does not support one.

The observed recall remained at 100%, meaning the revised prompt did not miss any of the seven known defects in this run.

The custom cases provided additional evidence that the improved prompt could identify a business-rule regression and avoid reporting a harmless refactoring as a defect.

## 8. Defect Categories Successfully Detected

| Category | Example | Case |
|---|---|---|
| Null handling | Nullable request collection dereferenced without validation | case-01 |
| Security | Untrusted parameters interpolated into SQL | case-02 |
| Business logic | Incorrect monetary rounding and discount calculation | case-03 |
| Authorization | Missing Admin role requirement on DELETE | case-05 |
| Data privacy | Logging complete invoice and session objects | case-07 |
| Concurrency | Replacing ConcurrentDictionary with Dictionary and non-atomic insertion | case-08 |
| Exception handling | Catch-all exception handler returns HTTP 200 | case-10 |
| Business-rule validation | Removing mandatory rejection-reason validation | custom-01 |
| Clean refactoring | Consistent local-variable rename | custom-02 |

The clean cases also tested the agent's ability to avoid unnecessary findings against harmless refactoring, tests, and documentation/configuration changes.

## 9. Limitations and Confidence

The results are encouraging, but several limitations must be considered:

1. The v2 evaluation on the original dataset contains only one run of 10 cases.
2. The two custom cases were each evaluated once.
3. The dataset consists of curated examples and may not represent all real-world code-review scenarios.
4. A 100% observed score on a small dataset does not establish general reliability.
5. AI model behavior can vary between sessions.
6. The evaluation does not measure finding severity, review time saved, or the percentage of findings accepted by human reviewers.
7. Additional runs and independently selected code changes are required to validate the improvement.

The baseline has three independent runs, while v2 has only one run on the original dataset. Therefore, the comparison is promising but not statistically conclusive.

### v2 Repeatability Limitation

The improved v2 prompt achieved 100% observed accuracy across the 10 evaluation cases in one run. However, unlike the baseline prompt, which was evaluated over three independent runs, v2 was evaluated only once. Therefore, the results are promising but insufficient to establish statistical confidence or consistent performance across repeated sessions. Additional independent runs are recommended to validate the improvement.

## 10. Conclusion

The baseline evaluation demonstrated strong defect detection, identifying all known defects across three independent runs, but produced two false alarms.

The improved v2 prompt maintained 100% observed recall while reducing false positives to zero in its single evaluation run.

The two additional custom cases also produced the expected outcomes:
- The agent detected the missing rejection-reason validation.
- The agent correctly classified a behavior-preserving variable rename as clean.

This suggests that prompt-level controls focused on evidence, realistic failure scenarios, and explicit false-positive prevention can improve AI-assisted code-review quality.

The agent should be used as an advisory reviewer rather than an autonomous approval mechanism. Human engineers remain responsible for validating findings, understanding business context, and approving code changes before merge.

## 11. Final Evaluation Status

| Evaluation component | Status |
|---|---|
| Baseline v1 — 3 runs, 30 reviews | Complete |
| Improved v2 — 10 original cases | Complete |
| Custom defect case | Complete |
| Custom clean case | Complete |
| Evaluation metrics | Complete |
| CSV results | Complete after duplicate removal |

**Final counts:**
- Original evaluation cases: 10
- Custom evaluation cases: 2
- Unique cases: 12
- Baseline reviews: 30
- Improved original-case reviews: 10
- Custom reviews: 2
- Total recorded evaluation results: 42

The CSV should contain exactly 42 unique data rows plus the header, after removing the duplicate Run 2 Case 03 entry.

**Deliverable 4 — Agent Evaluation: Complete.**