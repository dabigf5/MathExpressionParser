namespace MathExpressionParser;

public interface Expression;

public class BinaryExpr(Expression left, TokenType op, Expression right) : Expression {
    public readonly Expression Left = left;
    public readonly TokenType Op = op;
    public readonly Expression Right = right;
}

public class UnaryExpr(Expression subject, TokenType op) : Expression {
    public readonly Expression Subject = subject;
    public readonly TokenType Op = op;
}

public class NumberExpr(decimal number) : Expression {
    public readonly decimal Number = number;
}

public class VariableExpr(string name) : Expression {
    public readonly string Name = name;
}