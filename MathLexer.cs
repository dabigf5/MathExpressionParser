
using System.Text;

namespace MathExpressionParser;

internal class MathLexer(string text) {
    public readonly string Text = text;
    public int Position;
    
    public char Peek(int offset = 0) {
        return Text[Position + offset];
    }

    public void Advance(int amt = 1) {
        Position += amt;
    }
    
    public bool Finished() {
        return Position >= Text.Length;
    }

    public Token ReadNumber() {
        var builder = new StringBuilder();
        char ch;
        while (!Finished() && char.IsAsciiDigit(ch = Peek())) {
            builder.Append(ch);
            Advance();
        }

        return new Token(TokenType.Number, decimal.Parse(builder.ToString()));
    }

    public Token ReadIdentifier() {
        var builder = new StringBuilder();
        builder.Append(Peek());
        Advance();
        
        char ch;
        while (!Finished() && char.IsAsciiLetterOrDigit(ch = Peek())) {
            builder.Append(ch);
            Advance();
        }

        return new Token(TokenType.Identifier, builder.ToString());
    }
    
    public Token? ReadNextToken() {
        char ch = Peek();
        if (char.IsWhiteSpace(ch)) {
            Advance();
            return null;
        }
        
        if (char.IsAsciiLetter(ch)) {
            return ReadIdentifier();
        }
        
        if (char.IsAsciiDigit(ch)) {
            return ReadNumber();
        }
        
        Advance();
        return ch switch {
            '(' => new Token(TokenType.OpenPar),
            ')' => new Token(TokenType.ClosePar),
            '+' => new Token(TokenType.Plus),
            '-' => new Token(TokenType.Minus),
            '*' => new Token(TokenType.Star),
            '/' => new Token(TokenType.Slash),
            '^' => new Token(TokenType.Caret),
            ',' => new Token(TokenType.Comma),
            _ => throw new MathParseException($"Unexpected character '{ch}'")
        };
    }
    
    public List<Token> Lex() {
        List<Token> tokens = [];
        
        while (!Finished()) {
            var token = ReadNextToken();
            
            if (token != null)
                tokens.Add(token);
        }
        
        tokens.Add(new Token(TokenType.Eof));
        return tokens;
    }
    
    public static void PrintTokens(List<Token> tokens) {
        foreach (var token in tokens) {
            Console.Write(token.Type);
            if (token.Value != null) {
                Console.Write($": {token.Value}");
            }

            Console.WriteLine();
        }
    }
}