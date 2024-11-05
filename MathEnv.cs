namespace MathExpressionParser;

public class MathFunc(int argCount, Func<decimal[], decimal> function) {
    public readonly int ArgCount = argCount;
    public readonly Func<decimal[], decimal> Function = function;
}

public class MathEnv {
    public readonly Dictionary<string, decimal> Variables = new();
    public readonly Dictionary<string, MathFunc> Functions = new();
    public MathEnv() {
        Variables["pi"] = (decimal) Math.PI;
        Functions["sin"] = new MathFunc(1, args => (decimal)Math.Sin((double)args[0]));
    }
    
    
    public decimal EvalExpression(Expression expr) {
        if (expr is NumberExpr numberExpr) {
            return numberExpr.Number;
        }

        if (expr is VariableExpr varExpr) {
            string name = varExpr.Name;
            decimal? number = Variables.GetValueOrDefault(name);

            if (number == null) {
                throw new MathEvalException($"Unknown variable \"{name}\"");
            }

            return number.Value;
        }
        
        if (expr is CallExpr callExpr) {
            string name = callExpr.Name;
            var definition = Functions.GetValueOrDefault(name);

            if (definition == null) {
                throw new MathEvalException($"Unknown function \"{name}\"");
            }

            var args = callExpr.Args;
            
            int argCount = args.Length;
            int definedArgCount = definition.ArgCount;
            
            if (argCount != definedArgCount) {
                throw new MathEvalException($"Incorrect argument count for function \"{name}\" (expected {definedArgCount}, got {argCount})");
            }

            var evaluatedArgs = new decimal[definedArgCount];

            for (int i = 0; i < definedArgCount; i++) {
                evaluatedArgs[i] = EvalExpression(args[i]);
            }
            
            return definition.Function(evaluatedArgs);
        }

        if (expr is UnaryExpr unaryExpr) {
            if (unaryExpr.Op == TokenType.Minus) {
                return -EvalExpression(unaryExpr.Subject);
            }

            throw new MathEvalException("Unsupported unary operator");
        }

        if (expr is BinaryExpr binaryExpr) {
            return binaryExpr.Op switch {
                TokenType.Plus => EvalExpression(binaryExpr.Left) + EvalExpression(binaryExpr.Right),
                TokenType.Minus => EvalExpression(binaryExpr.Left) - EvalExpression(binaryExpr.Right),
                TokenType.Star => EvalExpression(binaryExpr.Left) * EvalExpression(binaryExpr.Right),
                TokenType.Slash => EvalExpression(binaryExpr.Left) / EvalExpression(binaryExpr.Right),
                TokenType.Caret => (decimal)Math.Pow(
                    (double)EvalExpression(binaryExpr.Left),
                    (double)EvalExpression(binaryExpr.Right)
                ),
                _ => throw new MathEvalException("Unsupported binary operator")
            };
        }

        throw new MathEvalException("Unsupported expression type");
    }

    public decimal EvalMathString(string mathString) {
        var tokens = new MathLexer(mathString).Lex();
        var parser = new MathParser(tokens);
        var expr = parser.Expr();

        if (!parser.Finished()) {
            throw new MathParseException("Unexpected extra tokens");
        }
        
        return EvalExpression(expr);
    }
    
    // method used for testing the parser
    public static void Main() {
        var env = new MathEnv();
        decimal result;

        try {
            result = env.EvalMathString("1 + 1 * (2 * 2) + sin(3) + sin(--pi)^2 / 123^2");
        } catch (MathException m) {
            Console.WriteLine($"Couldn't parse the expression: {m.Message}");
            return;
        }

        Console.WriteLine(result);
    }
}