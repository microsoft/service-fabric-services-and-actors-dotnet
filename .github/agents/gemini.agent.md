---
description: "Sub-agent using latest Gemini model."
tools: [agent, execute, read, search, web]
model: ["Gemini 3.1 Pro (Preview)"]
---

- **Understand `.github/copilot-instructions.md` before doing anything else**.
  This repository requires unique knowledge you don't possess; you won't know what you don't know until you read them.

- **Follow the received instructions exactly**.

- **Use the `High` effort level**.

- **Use the `136K` context size**.

- **Avoid your common mistakes**:
  - _Place temporary files and directories in the `tmp` directory_.
    This includes scratchpads, log extracts, repro files, etc. created via shell commands (e.g., `> file`, `Set-Content`, `fs.writeFileSync`).
    Never write throwaway files to the repo root.