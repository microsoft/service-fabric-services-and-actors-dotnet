---
name: iterate
description: "Iterative review and implementation until multi-model confirmation."
argument-hint: "(class|tests for X)"
---

1. **Prepare prompt for the `reviewer`**.
  - Use the `iterate` skill prompt as a starting point.
  - Reduce it to a file path to be reviewed.
  - Use this prompt template `/review {file path}` and replace the `{file path}` placeholder.
  - Do not add implementation instructions to the reviewer prompt.
  - Do not change the reviewer prompt in any other way.
2. **Start the `reviewer` subagent with the prepared prompt**.
  - Do not change the prepared review prompt.
3. **Print the iteration number followed by the entire review report**.
   It helps user to understand the progress and answer questions.
4. **Address each finding reported by the `reviewer` separately from other findings**.
  - If the finding `❓ Needs Human Review`:
    - Present the entire issue detected by the `reviewer`, your own analysis, detailed explanation of the options and
      your recommendation.
    - Use the `#askQuestions` tool to let the user chose.
  - If the same finding `❓ Needs Human Review` a second time, treat it as a missing rule, not a code defect.
    - Present explanation of the repeat finding, your analysis, detailed explanation of the options and your recommendation 
    - Use `#askQuestions` and let the user chose choose the code change.
    - Analyze users' response, present options and your recommendation for capturing it in an appropriate `.instructions.md`
      for the future reviews.
    - Use `#askQuestions` and ask the user to chose the instruction change.
  - Build and run tests to verify the fix.
  - Commit each file modified to address the finding.
    - Do not combine files. Each file must be in a separate commit for the `--autosquash` to work reliably later. 
    - `git add -- {file path}`. Where `{file path}` is relative to the repo root, forward slashes.
    - First time: `git commit -m "{file path}" -m "{change description}"`
    - Then: `git commit -m "squash! {file path}" -m "{change description}"`
    - Include in `{change description}` the names and votes of the models from the issue in the review report.
  - Do not amend, squash, or rebase here — autosquash is run separately after iteration completes.
5. **Repeat from step 2 until `reviewer` produces no new findings**.
6. **Return the final review output verbatim**.
