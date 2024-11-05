# MathExpressionParser

A math expression parser written in C# that supports operator precedence, parenthesis, variables, and functions.

Operators (in order of precedence):
- `^` for exponentiation
- `-` for unary negation
- `*` for multiplication
- `/` for division
- `+` for addition
- `-` for subtraction

I don't quite understand how .NET libraries work, so if you want to use this parser for whatever reason, just copy the code files over or something.
## Examples

Solving 1+1:
```csharp
var env = new MathEnv();
decimal result;

try {
    result = env.EvalMathString("  1+ 1"); // spaces are ignored
} catch (MathException m) {
    Console.WriteLine($"Couldn't parse the expression: {m.Message}");
    return;
}

Console.WriteLine($"The result is {result}"); // "The result is 2"
```

Using functions and variables:
```csharp
var env = new MathEnv();
decimal result;

try {
    result = env.EvalMathString("cos(pi)");
} catch (MathException m) {
    Console.WriteLine($"Couldn't parse the expression: {m.Message}");
    return;
}

Console.WriteLine($"The result is {result}"); // "The result is -1"
```

Defining your own functions and variables for expressions to make use of:
```csharp
var env = new MathEnv();
decimal result;

env.Variables["myFavoriteNumber"] = 123m;
env.Functions["half"] = new MathFunc(1, args => args[0] / 2;

try {
    result = env.EvalMathString("half(myFavoriteNumber)");
} catch (MathException m) {
    Console.WriteLine($"Couldn't parse the expression: {m.Message}");
    return;
}

Console.WriteLine($"The result is {result}"); // "The result is 61.5"
```