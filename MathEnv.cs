namespace MathExpressionParser;

public abstract class MathException(string message) : Exception(message);
public class MathParseException(string message) : MathException(message);
public class MathEvalException(string message) : MathException(message);

public class MathEnv {
    public readonly Dictionary<string, object> Variables = new();
    public MathEnv() {
        Variables["pi"] = (decimal) Math.PI;
    }
    
    
    public decimal EvalExpression(Expression expr) {
        if (expr is NumberExpr numberExpr) {
            return numberExpr.Number;
        }

        if (expr is VariableExpr varExpr) {
            string name = varExpr.Name;
            object? definition = Variables.GetValueOrDefault(name);

            if (definition == null) {
                throw new MathEvalException($"Unknown variable \"{name}\"");
            }
            
            if (definition is decimal number) {
                return number;
            }

            throw new MathEvalException($"Attempt to evaluate non-number variable \"{name}\" as a number");
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
            result = env.EvalMathString("pi()");
        } catch (MathException m) {
            Console.WriteLine($"Couldn't parse the expression: {m.Message}");
            return;
        }

        Console.WriteLine(result);
    }
}