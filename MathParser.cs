namespace MathExpressionParser;

internal class MathParser(List<Token> tokens) {
    public readonly List<Token> Tokens = tokens;
    public int Position;

    public bool Finished() {
        return Check(TokenType.Eof);
    }
    
    private Token Peek(int offset = 0) {
        return Tokens[Position + offset];
    }

    private void Advance(int amount = 1) {
        Position += amount;
    }

    private bool Check(params TokenType[] acceptables) {
        return acceptables.Contains(Peek().Type);
    }
    private bool Match(params TokenType[] acceptables) {
        bool accepted = Check(acceptables);
        
        if (accepted) {
            Advance();
        }

        return accepted;
    }

    public Expression[] ExprList() {
        var expressions = new List<Expression>();
        
        Advance(); // open par
        do {
            var expr = Expr();
            expressions.Add(expr);
        } while (Match(TokenType.Comma));

        if (!Match(TokenType.ClosePar)) {
            throw new MathParseException("Expected )");
        }

        return expressions.ToArray();
    }
    
    public Expression Expr() {
        return Subtraction();
    }
    
    public Expression Subtraction() {
        var left = Addition();

        while (Match(TokenType.Minus)) {
            var right = Addition();
            left = new BinaryExpr(left, TokenType.Minus, right);
        }

        return left;
    }
    
    public Expression Addition() {
        var left = Division();

        while (Match(TokenType.Plus)) {
            var right = Division();
            left = new BinaryExpr(left, TokenType.Plus, right);
        }

        return left;
    }
    public Expression Division() {
        var left = Multiplication();

        while (Match(TokenType.Slash)) {
            var right = Multiplication();
            left = new BinaryExpr(left, TokenType.Slash, right);
        }

        return left;
    }
    public Expression Multiplication() {
        var left = Unary();

        while (Match(TokenType.Star)) {
            var right = Unary();
            left = new BinaryExpr(left, TokenType.Star, right);
        }

        return left;
    }
    public Expression Unary() {
        if (Match(TokenType.Minus)) {
            return new UnaryExpr(Unary(), TokenType.Minus);
        }

        return Exponent();
    }
    public Expression Exponent() {
        var left = Primary();

        if (Match(TokenType.Caret)) {
            return new BinaryExpr(left, TokenType.Caret, Exponent());
        }

        return left;
    }
    
    public Expression Primary() {
        if (Match(TokenType.Number)) {
            var token = Peek(-1);
            return new NumberExpr((decimal) token.Value!);
        }

        if (Match(TokenType.Identifier)) {
            string id = (string) Peek(-1).Value!;
            
            if (Check(TokenType.OpenPar)) {
                return new CallExpr(id, ExprList());
            }
            
            return new VariableExpr(id);
        }
        
        if (Match(TokenType.OpenPar)) {
            var nestedExpr = Expr();
            if (!Match(TokenType.ClosePar)) {
                throw new MathParseException("Expected )");
            }
            return nestedExpr;
        }

        throw new MathParseException("Unexpected token");
    }
}