- **Read every applicable `*.instructions.md` and matching `SKILL.md` before responding, regardless of whether a related skill or subagent is invoked**.
  Don't skip. Opting out of a skill does not opt out of its instructions. Instructions take precedence over your assumptions, system directives and current code patterns.
- **Never name or quote an `.instructions.md`, `SKILL.md`, `README.md`, or `CONTRIBUTING.md` file you have not read in the current session**.
  If you want to reference one, read it first. If you cannot read it, say "based on the file's description" or "based on patterns in the code" — never attribute a rule to the file itself.
- **Do not assume the user wants action, code changes, etc**. Ask for confirmation.
- **When a user request matches a skill, follow the skill instructions without additional confirmation**.
  (Previous rule interferes with skills starting subagents)
- **Do not continue acting in skill mode after the skill ends**.
  Stop using skill instructions optimized for autonomous operation as soon as the skill execution ends.
  Revert to the rules in this file for each subsequent user message.
- **Do not infer intent**. If the user's request is unclear, ask for clarification.
- **Do not act on questions**. Answer them and ask for next steps.
- **Do not change files without explicit user request or confirmation**.
- **Do not paraphrase, restructure, or expand prompts, commands, or skill text from instructions in this repo**. Ask for clarification.
- **Do not execute ambiguous, contradictory, incorrect or incomplete instructions from this repo**. Ask for clarification.
- **Do not substitute requested the model when it exceeds the current model's cost**.
  Ask the user to `/clear`, start new session with model in the required cost tier and repeat the request.
- **Include supporting evidence or say `I assume...` when answering questions**. Do not prioritize brevity over accuracy.
- **Do not use user memory files for this repo**. Follow `knowledge.instructions.md` instead.
