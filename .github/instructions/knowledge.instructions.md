---
description: "Always use before reasoning about, modifying or reviewing any files in this repo."
---

# Understand Project Context

Find and read the `README.md` and `CONTRIBUTING.md` files closest to the files being modified or reviewed. Walk up from the file's directory to the repository root, stopping at the first match for each.

# Capture Insights from Feedback

After applying any change requested through feedback, check whether the feedback contains a reusable insight before responding. This is a required final step, not a background task.

1. Scan the user's message for signal phrases: `note`, `always`, `never`, `make sure`, `you should've`, `why didn't you`, or repeated corrections of the same kind.
2. If a reusable guideline is present, update the appropriate `.md` file as part of the same response — don't defer it.
3. Don't codify trivial one-off corrections, only high-confidence insights.
4. Ask for clarification and confirmation when in doubt.

# Organize Knowledge for Humans

- Your audience is experienced engineers. Don't restate the obvious.
- Avoid duplicating information readily available from the source code.
- Store information in the most specific `.md` files possible. Elevate it to more general `.md` files only when confident it applies to wider scope.
- Store human-readable insights and guidelines in the `CONTRIBUTING.md` files.
  - Place repository-wide content in the `CONTRIBUTING.md` file in the root directory.
  - When repository contains multiple product projects (as opposed to test or example projects), place project-specific content the project directory.
- Store instructions applicable across multiple repositories, such as coding style, methodology, etc. in the `.instructions.md` files.
- Avoid storing information in user-specific `memories`. You're helping teams, not individuals.
- Keep `.instructions.md` files focused on specific topics.
- Check `.md` files for accuracy and redundancy during code reviews.

# Ensure Knowledge Available to Agents

Make sure `copilot-instructions.md` contains minimal instructions required for agents to discover the available knowledge.
- Instructions to checkout submodules checkout, if any.
- Instructions to read `README.md` and `CONTRIBUTING.md` files.

# Checklist

- When adding code or content check if it already exists elsewhere, and extract it into a shared location instead of copying it.
- Update `.md` files when modifying code artifacts referenced in them, such as functions, classes, packages, etc.
