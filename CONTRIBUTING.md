# Contributing

DiceEngine.NET is an open-source project, and as such, any contributions are appreciated. Please use the following as guidelines for contributions to the project.

## Development Cycle

Please, whenever you can, provide discussion about changes before opening a pull request. This can be done through opening a GitHub issue. Two templates are provided, one for a bug report and one for a feature request.

Issues that are tagged as "Up for grabs" are free to be picked up and worked on by anyone.

### Pull Requests

Pull requests should include a description of the changes that are being made. They aren't required, but will make reviewing the pull request easier. Please also link the PR to the related issue. For now, each pull request is reviewed automatically by [CodeRabbit](https://www.coderabbit.ai/). The automatic AI reviews may change later if this repository gains more activity.

## Semantic Versioning

This project tries to adhere to [Semantic Versioning](http://semver.org/). When writing changes to this project, please try to avoid any breaking changes whenever possible. If a breaking change is not avoidable, a pull request or feature may become delayed.

The working release is based off of the `dev` branch.

We follow the .NET Foundation's [Breaking Change Rules](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/breaking-change-rules.md) when determining the SemVer compliance of a change.

## Coding Style

Most of the code follows the .NET Foundation's [Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) where possible.

As a general rule, follow the coding style already set in the file you are editing, or look at a similar file if you are adding a new one. (Variable names using snake_case are being replaced with cammelCase variable names.)

### Documentation Style

When creating a new public member, the member must be annotated with sufficient documentation. This should include the
following, but not limited to:

- `<summary>` summarizing the purpose of the method.
- `<param>` or `<typeparam>` explaining the parameter.
- `<return>` explaining the type of the returned member and what it is.
- `<exception>` if the method directly throws an exception.

## Getting Started

### Setting Up Your Development Environment

1. Fork the repository from the dev branch
1. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/DiceEngine.NET.git
   cd DiceEngine.NET
   git checkout dev
   ```
1. Add the upstream repository as a remote:
   ```bash
   git remote add upstream https://github.com/andrewk17111/DiceEngine.NET.git
   ```
1. In Visual Studio or your preferred C# IDE, build the solution:
   ```bash
   dotnet restore
   dotnet build
   ```

### Making Changes

1. Create a new branch for your feature/fix from the `dev` branch:
   ```bash
   git checkout dev
   git pull upstream dev
   git checkout -b your-branch-name
   ```
1. Make your changes following the coding style guidelines
1. Write or update relevant unit tests
1. Update documentation as needed
1. Commit your changes with clear commit messages

## Testing Guidelines

### Unit Tests

The project uses MSTest for unit testing. When adding new functionality:

1. Create or update tests in the corresponding test project
1. Name tests clearly
1. Include tests for:
   - Normal operation
   - Edge cases
   - Error conditions
1. Run all tests locally before submitting a PR using the `dotnet` command or run the tests in Visual Studio:
   ```bash
   dotnet test
   ```

## Questions and Support

If you have questions about contributing:

1. Check existing issues and documentation
1. Open a new issue with the "Question" label
1. Provide as much context as possible about your question

Thank you for contributing to DiceEngine.NET!
