namespace MathExpressionParser;

public abstract class MathException(string message) : Exception(message);
public class MathParseException(string message) : MathException(message);
public class MathEvalException(string message) : MathException(message);

public static class MathEvaluator {
    public static void PrintTokens(List<Token> tokens) {
        foreach (var token in tokens) {
            Console.Write(token.Type);
            if (token.Value != null) {
                Console.Write($": {token.Value}");
            }

            Console.WriteLine();
        }
    }

    public static decimal EvalExpression(Expression expr) {
        if (expr is NumberExpr numberExpr) {
            return numberExpr.Number;
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

    public static decimal EvalMathString(string mathString) {
        var tokens = new MathLexer(mathString).Lex();
        var expr = new MathParser(tokens).Expr();
        return EvalExpression(expr);
    }
    
    // method used for testing the parser
    public static void Main() {
        decimal result;

        try {
            result = EvalMathString("2^(2*2 + 4) - 1");
        } catch (MathException m) {
            Console.WriteLine($"Couldn't parse the expression: {m.Message}");
            return;
        }

        Console.WriteLine(result);
    }
}