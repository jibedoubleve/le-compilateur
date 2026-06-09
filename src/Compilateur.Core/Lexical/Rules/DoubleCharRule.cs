using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public abstract record DoubleCharRule : ITokenRule
{
    #region Fields

    private readonly string _lexeme;
    private readonly TokenType _tokenType;

    #endregion

    #region Constructors

    public DoubleCharRule(string lexeme, TokenType tokenType)
    {
        if (lexeme.Length != 2)
            throw new ArgumentOutOfRangeException(
                $"The lexeme should have a length of 2 but length is {lexeme.Length}"
            );

        Weight = lexeme.Length;
        _lexeme = lexeme;
        _tokenType = tokenType;
    }

    #endregion

    #region Properties

    public int Weight { get; }

    #endregion

    #region Methods

    public bool Matches(CodeStream codeStream)
    {
        var current = $"{codeStream.Peek()}{codeStream.PeekNext()}";
        return _lexeme == current;
    }

    public Token Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var first = codeStream.Consume();
        var second = codeStream.Consume();

        return new Token
        {
            Lexeme = $"{first.Char}{second.Char}",
            Type = _tokenType,
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
public sealed record AndRule() : DoubleCharRule("&&", TokenType.And);

public sealed record OrRule() : DoubleCharRule("||", TokenType.Or);

public sealed record GreaterOrEqualRule() : DoubleCharRule(">=", TokenType.GreaterOrEqual);

public sealed record LessThanOrEqualRule() : DoubleCharRule("<=", TokenType.LessThanOrEqual);

public sealed record EqualityRule() : DoubleCharRule("==", TokenType.Equality);

public sealed record InequalityRule() : DoubleCharRule("!=", TokenType.Inequality);