---
description: "Guidelines for PowerShell scripts."
applyTo: "**/*.ps1"
---

- **Read the `coding.instructions.md` first**.
  Instructions in this file are incomplete without them.

# Script structure

- Place the main logic in a `Main` function near the top of the file so the high-level flow is immediately visible.
- Define helper functions after `Main`, and call `Main` at the end.
- Validate script parameters before `Main`, not inside it.
- Initialize script-scope variables before `Main` so their availability to all functions is obvious.
- Keep functions at consistent level of abstraction. Extract lower-level logic into other functions.
- Extract functions shared by multiple scripts into a common helper file and dot-source it.
- Dot-source helper scripts at the beginning, right after `param`, strict mode, and error preference.
- Minimize duplication, including repeated substrings in expressions (e.g., factor out a common suffix when constructing paths).

# Function naming

- Use simple descriptive names for local functions (e.g., `AddNuGetPackageSource`).
- Don't use PowerShell Verb-Noun cmdlet naming convention for local functions.
- Use Verb-Noun naming for functions designed to be invoked interactively by users (e.g., `Get-Packages`).

# Variable and parameter naming

- Script/cmdlet parameters and script-scope variables: PascalCase (e.g., `$RuntimeRoot`, `$script:SdkPackages`).
- Function parameters and local variables: camelCase (e.g., `$configPath`, `$packagesPath`).
- Always specify the type explicitly when declaring a variable, with a space between the type and the variable (e.g., `[string] $path`, `[int] $count`, `[string[]] $names`).

# Strings

- Use single quotes for literal strings.
- Use double quotes only for interpolated strings.

# File paths

- Always resolve file paths to canonical form (no `..` or `.` segments) before using them in output, config files, or error messages.

# Scripts manipulating source code

- Preserve original formatting, whitespace and comments, particularly when modifying XML- or JSON-based based files.
- Use text manipulation (string replacement, regex) instead of DOM APIs (e.g., `[xml]`, `XmlDocument`).
