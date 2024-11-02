# MathExpressionParser

A math expression parser written in C# that supports operator precedence and parenthesis.

Supported operators:
- `+` for addition
- `-` for subtraction
- `*` for multiplication
- `/` for division
- `^` for exponentiation

I don't quite understand how .NET libraries work, so if you want to use this parser for whatever reason, just copy the code files over or something.
## Usage

```csharp
decimal result;

try {
    result = MathEvaluator.EvalMathString("  1+ 1"); // spaces are ignored
} catch (MathException m) {
    Console.WriteLine($"Couldn't parse the expression: {m.Message}");
    return;
}

Console.WriteLine($"The result is {result}"); // "The result is 2"
```