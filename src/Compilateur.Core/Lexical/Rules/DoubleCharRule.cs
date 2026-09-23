using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public abstract record DoubleCharRule : ITokenRule
{
    #region Fields

    private readonly string _lexeme;
    private readonly TokenKind _tokenKind;

    #endregion

    #region Constructors

    public DoubleCharRule(string lexeme, TokenKind tokenKind)
    {
        if (lexeme.Length != 2)
        {
            throw new ArgumentOutOfRangeException(
                $"The lexeme should have a length of 2 but length is {lexeme.Length}"
            );
        }

        Weight = lexeme.Length;
        _lexeme = lexeme;
        _tokenKind = tokenKind;
    }

    #endregion

    #region Properties

    public int Weight { get; }

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor)
    {
        var current = $"{codeCursor.Peek()}{codeCursor.PeekNext()}";
        return _lexeme == current;
    }

    public Token Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var first = codeCursor.Consume();
        var second = codeCursor.Consume();

        return new Token
        {
            Lexeme = $"{first.Char}{second.Char}",
            Kind = _tokenKind,
            Value = null,
            Column = first.Column,
            Line = first.Line
        };
    }

    #endregion
}

/* =========================
 * Double char rules
 * ========================= */
public sealed record GreaterOrEqualRule() : DoubleCharRule(">=", TokenKind.GreaterThanOrEqual);

public sealed record LessThanOrEqualRule() : DoubleCharRule("<=", TokenKind.LessThanOrEqual);

public sealed record EqualityRule() : DoubleCharRule("==", TokenKind.Equality);

public sealed record InequalityRule() : DoubleCharRule("!=", TokenKind.Inequality);