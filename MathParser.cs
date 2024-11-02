namespace MathExpressionParser;

internal class MathParser(List<Token> tokens) {
    public readonly List<Token> Tokens = tokens;
    public int Position;
    
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

    private void Expect(TokenType type, string errorMessage) {
        var token = Peek();
        bool accepted = token.Type == type;

        if (!accepted) {
            throw new MathParseException(errorMessage);
        }
        
        Advance();
    }
    
    public Expression Expr() {
        return Term();
    }
    
    private Expression Term() {
        var expr = Factor();

        while (Match(TokenType.Plus, TokenType.Minus)) {
            var op = Peek(-1).Type;
            var right = Factor();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }
    
    private Expression Factor() {
        var expr = Unary();

        while (Match(TokenType.Slash, TokenType.Star)) {
            var op = Peek(-1).Type;
            var right = Unary();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }
    
    private Expression Unary() {
        if (Match(TokenType.Minus)) {
            return new UnaryExpr(Unary(), Peek(-1).Type);
        }

        return Exponent();
    }
    
    private Expression Exponent() {
        var expr = Primary();

        while (Match(TokenType.Caret)) {
            var op = Peek(-1).Type;
            var right = Unary();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }
    
    private Expression Primary() {
        var token = Peek();

        if (token.Type == TokenType.Identifier) {
            throw new MathParseException("Variables are not yet implemented!");
        }
        
        if (token.Type == TokenType.Number) {
            Advance();
            return new NumberExpr((decimal) token.Value!);
        }

        if (token.Type == TokenType.OpenPar) {
            Advance();
            var expr = Expr();
            Expect(TokenType.ClosePar, "Expected )");
            return expr;
        }

        throw new MathParseException("Unexpected token");
    }
}