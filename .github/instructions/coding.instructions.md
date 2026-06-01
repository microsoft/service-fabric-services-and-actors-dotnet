---
description: "Guidelines for all code or content in this repo."
applyTo: "**"
---

- **Read the `knowledge.instructions.md` first**.
  Instructions in this file are incomplete without them.

# Coding Guidelines

- Follow these principles unless overridden by language-specific rules
  - [Beck's Rules of Simple Design](https://martinfowler.com/bliki/BeckDesignRules.html)
  - [Robert Martin's SOLID Design Principles](https://en.wikipedia.org/wiki/SOLID)
  - [Jim Shore's Fail Fast Principle](https://martinfowler.com/ieeeSoftware/failFast.pdf)
  - [Jeff Atwood's Rule of Three](https://blog.codinghorror.com/rule-of-three/)

- Read and follow the guidelines codified in the `.editorconfig` before writing or reviewing code.

## Limit scope of changes

- Apply coding style rules to new code and code being substantially modified, not to every file touched.
- When making a targeted change (e.g. adding a header, fixing a bug), don't reformat the surrounding code.

## Make the code as informative and intuitive as possible

- Folder structure within a project should match the namespace or module structure.
- Define top-level types in separate files with a matching name.

## Make the code as concise as possible

- Don't use unnecessary braces.
- Before adding braces, try to make them unnecessary by extracting a function, a class, etc.
- Shorten parameter and variable names to the minimum needed to understand them in context.
- Don't add comments re-stating the information already available from the declaration.
- Before adding comments, try to make them unnecessary by breaking up the code into multiple functions, classes, etc.
- Wrap text lines at the first word boundary after column 120. Typical lines will be longer.
- Don't create a container (namespace, folder, section) until you have 3 or more items to justify its overhead.
