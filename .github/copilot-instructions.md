# GitHub Copilot Instructions for Service Fabric .NET SDK

- Run `init.cmd` (Windows) or `init.sh` (Linux) before the first build.
- Build and test with `-c Release` to avoid known `Debug` test failures.
- Build and test specific projects to reduce execution time.
- Run tests with `-f net472` and/or `-f net10.0` to speed up change verification.
- Run all tests on all frameworks before considering the change completed.
- After each feedback, check if it needs to be captured as new [knowledge](./instructions/knowledge.instructions.md).
- See [CONTRIBUTING.md](../CONTRIBUTING.md) for build commands, project structure, and contribution guidelines.
- See `README.md` files in the root and project directories for high-level overview.
