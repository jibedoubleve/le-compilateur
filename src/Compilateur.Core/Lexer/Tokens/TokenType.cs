namespace Compilateur.Core.Lexer.Tokens;

public enum TokenType
{
    // One char lexemes
    Dot,
    Comma,
    Semicolon,
    OpenParenthesis,
    CloseParenthesis,
    OpenCurlyBracket,
    ClosingCurlyBracket,
    Bang,
    GreaterThan,
    LessThan,
    Assignment,
    Plus,
    Minus,
    Multiply,
    Divided,

    // Two char lexemes
    And,
    Or,
    GreaterOrEqual,
    LessThanOrEqual,
    Equality,
    Inequality,

    // Multiple char lexemes
    Identifier,
    Numeric,
    String,
    True,
    False,
    Nil,
    If,
    Else,
    While,
    For,
    Fun,
    Return,
    Class,
    This,
    Super,
    Var,
    Print,

    // Special tokens
    Eof
}