using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public abstract record SingleCharRule : ITokenRule
{
    #region Fields

    private readonly char _lexeme;
    private readonly TokenType _tokenType;

    #endregion

    #region Constructors

    public SingleCharRule(char lexeme, TokenType tokenType)
    {
        _lexeme = lexeme;
        _tokenType = tokenType;
    }

    #endregion

    #region Properties

    public int Weight => 1;

    #endregion

    #region Methods

    public bool Matches(CodeStream codeStream) => codeStream.Peek() == _lexeme;

    public Token Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var consumed = codeStream.Consume();
        if (consumed.Char is null) throw new InvalidOperationException("Consumed empty lexeme");

        return new Token
        {
            Column = consumed.Column,
            Line = consumed.Line,
            Lexeme = consumed.Char!.ToString()!,
            Type = _tokenType,
            Value = null
        };
    }

    #endregion
}

/* =========================
 * Single char rules
 * ========================= */
public sealed record DotRule() : SingleCharRule('.', TokenType.Dot);

public sealed record CommaRule() : SingleCharRule(',', TokenType.Comma);

public sealed record SemiColonRule() : SingleCharRule(';', TokenType.Semicolon);

public sealed record OpenBracketRule() : SingleCharRule('(', TokenType.OpenParenthesis);

public sealed record ClosingBracketRule() : SingleCharRule(')', TokenType.CloseParenthesis);

public sealed record OpenCurlyBracketRule() : SingleCharRule('{', TokenType.OpenCurlyBracket);

public sealed record ClosingCurlyBracketRule() : SingleCharRule('}', TokenType.ClosingCurlyBracket);

public sealed record BangRule() : SingleCharRule('!', TokenType.Bang);

public sealed record GreaterThanRule() : SingleCharRule('>', TokenType.GreaterThan);

public sealed record LessThanRule() : SingleCharRule('<', TokenType.LessThan);

public sealed record AssignmentRule() : SingleCharRule('=', TokenType.Assignment);

public sealed record PlusRule() : SingleCharRule('+', TokenType.Plus);

public sealed record MinusRule() : SingleCharRule('-', TokenType.Minus);

public sealed record MultiplyRule() : SingleCharRule('*', TokenType.Multiply);

public sealed record DividedRule() : SingleCharRule('/', TokenType.Divided);