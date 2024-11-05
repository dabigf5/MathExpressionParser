namespace MathExpressionParser;

public enum TokenType {
    OpenPar,
    ClosePar,
    Comma,
    
    Plus,
    Minus,
    Slash,
    Star,
    Caret,
    
    Number,
    Identifier,
    
    Eof,
}

public class Token(TokenType type, object? value = null) {
    public readonly TokenType Type = type;
    public readonly object? Value = value;
}