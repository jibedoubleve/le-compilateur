using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public abstract record SingleCharRule : ITokenRule
{
    #region Fields

    private readonly char _lexeme;
    private readonly TokenKind _tokenKind;

    #endregion

    #region Constructors

    public SingleCharRule(char lexeme, TokenKind tokenKind)
    {
        _lexeme = lexeme;
        _tokenKind = tokenKind;
    }

    #endregion

    #region Properties

    public int Weight => 1;

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor) => codeCursor.Peek() == _lexeme;

    public Token Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var consumed = codeCursor.Consume();
        if (consumed.Char is null)
        {
            throw new InvalidOperationException("Consumed empty lexeme");
        }

        return new Token
        {
            Column = consumed.Column,
            Line = consumed.Line,
            Lexeme = consumed.Char!.ToString()!,
            Kind = _tokenKind,
            Value = null
        };
    }

    #endregion
}

/* =========================
 * Single char rules
 * ========================= */
public sealed record DotRule() : SingleCharRule('.', TokenKind.Dot);

public sealed record CommaRule() : SingleCharRule(',', TokenKind.Comma);

public sealed record SemiColonRule() : SingleCharRule(';', TokenKind.Semicolon);

public sealed record OpenBracketRule() : SingleCharRule('(', TokenKind.OpenParenthesis);

public sealed record ClosingBracketRule() : SingleCharRule(')', TokenKind.CloseParenthesis);

public sealed record OpenCurlyBracketRule() : SingleCharRule('{', TokenKind.OpenCurlyBracket);

public sealed record ClosingCurlyBracketRule() : SingleCharRule('}', TokenKind.CloseCurlyBracket);

public sealed record BangRule() : SingleCharRule('!', TokenKind.Bang);

public sealed record GreaterThanRule() : SingleCharRule('>', TokenKind.GreaterThan);

public sealed record LessThanRule() : SingleCharRule('<', TokenKind.LessThan);

public sealed record AssignmentRule() : SingleCharRule('=', TokenKind.Assignment);

public sealed record PlusRule() : SingleCharRule('+', TokenKind.Plus);

public sealed record MinusRule() : SingleCharRule('-', TokenKind.Minus);

public sealed record MultiplyRule() : SingleCharRule('*', TokenKind.Multiply);

public sealed record DividedRule() : SingleCharRule('/', TokenKind.Divided);