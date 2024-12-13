# Getting Started
## Prerequisites
To use DiceEngine.NET, you need to have:
- .NET Core 8 or higher
- Visual Studio, Visual Studio Code, or another code editor

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

## Basic Usage
Once you have installed DiceEngine.NET, you can start using it to parse and evaluate expressions. Here's a simple example:

```
csharp
using DiceEngine.Parsing;

public class Program
{
    public static void Main()
    {
        // Create a new math parser instance with default options.
        Parser parser = new Parser();

        // Parse and Evaluate an expression.
        IExpression result = parser.Parse("2 * -10 + 3").Evaluate();

        Console.WriteLine("Result: " + result);
        // Displays Result: -17
    }
}
```
This code snippet demonstrates how to parse and evaluate a mathematical expression using DiceEngine.NET.