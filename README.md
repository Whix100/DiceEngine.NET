# DiceEngine.NET

![Lines of Code](https://tokei.rs/b1/github/whix100/DiceEngine.NET?style=flat-square)
[![License](https://img.shields.io/github/license/whix100/DiceEngine.NET?style=flat-square)](https://github.com/whix100/DiceEngine.NET/blob/dev/LICENSE)
[![Open Issues](https://img.shields.io/github/issues/whix100/DiceEngine.NET?style=flat-square)](https://github.com/whix100/DiceEngine.NET/issues)
[![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/whix100/DiceEngine.NET?style=flat-square)](https://github.com/whix100/DiceEngine.NET/pulls)

A .NET 8 dice notation expression parser and calculator library

DiceEngine.NET is a versatile and efficient dice notation expression parsing library developed in C# targeting .NET 8. This library allows you to parse strings containing dice notation expressions into expression trees and then evaluated to calculate the resulting value.

## Features

- Expression Parsing: Parse and tokenize mathematical expressions and dice notation provided as strings.
- Unary Operations: Perfoms unary operations supporting prefixes and postfixes.
- Binary Operations: Perform arithmetic operations such as addition, subtraction, multiplication, division, integer division, and modulus.
- Parentheses Support: Handle nested expressions using parentheses for accurate calculations.
- Variables: Store values in variables and use them in expressions.
- Constants: Store predefined values to be used in expressions.
- Functions: Define and use custom functions using lambda expressions, along with built-in functions in expressions.
  - Conditional Function Attributes: Define conditions that parameters must meet before being passed to a function.
  - Preprocess Function Attributes: Define preprocesses that are applied to a parameter before being passed to a function.
- Collections: Expressions that are groups containing other functions
  - Vectors: Vectors can contain any number of elements in a set order, allowing for non-unique elements.
  - Sets: Sets can contain any number of unique elements in an indeterminate order.
- Indexers: An expression to get an element from another expression

### Dice Features
- Roll Result: Roll results store the results of the rolls from dice. These results will then be summed together when evaluated.
- Die: A die generates a random value between 1 and the max value of the die.
- Percentile Die: A percentile die generates a random value between 0 and 90 that is a multiple of 10.

## Installation

### From NuGet Using Visual Studio

1. Create or Open your solution/project
1. In the Solution Explorer, find the "Dependencies" item in your project
1. Right click "Dependencies" and select "Manage NuGet packages..."
1. In the "Browse" tab, search for "DiceEngine.NET"
1. Install the "DiceEngine.NET" package

### From NuGet Using dotnet CLI

1. Launch the terminal of your choice
1. Navigate to where your csproj file is located
1. Enter `dotnet add package DiceEngine.NET`

### From GitHub Releases

1. Go to the Releases section of this repo
1. Find the version you want
1. Download the DiceEngine.dll file from that release
1. Add the file as a reference to your project

## Usage

Here's a simple example demonstrating how to use DiceEngine.NET:

```csharp
using DiceEngine.Parsing;

public class Program
{
    public static void Main()
    {
        // Create a new math parser instance.
        Parser parser = new Parser();

        // Evaluate an expression.
        IExpression result = parser.Parse("2 * -10 + 3").Evaluate();

        Console.WriteLine("Result: " + result);
        // Displays Result: -17
    }
}
```

Alternatively, a Tokenizer can be provided to specify tokenizing behavior or new parsing rules can be supplied to to modify parsing behavior.

## Contributing

We welcome contributions to DiceEngine.NET! Whether it's bug fixes, new features, or improvements to the documentation, your help is greatly appreciated. More information can be found in this file repository's [CONTRIBUTING.md](https://github.com/andrewk17111/DiceEngine.NET/blob/dev/CONTRIBUTING.md) file.

### How to Contribute

1. **Fork the Repository**: Create a fork of this repository.
1. **Create a Branch**: Make a new branch for your changes based on the `dev` branch.
1. **Make Your Changes**: Implement your changes and commit them with clear and descriptive commit messages.
1. **Submit a Pull Request**: Open a pull request to the `dev` branch of the main repository, detailing the changes you made and why.

## License

DiceEngine.NET is released under the MIT License.

## Branches

### Release/X.X

Release branch following Major.Minor. Upon release, patches will be pushed to these branches and tagged.
New NuGet releases will be tagged on these branches.

### Dev

Development branch. This branch is what pull requests are targetted to.
