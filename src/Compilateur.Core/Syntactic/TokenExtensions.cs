using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic;

public static class TokenExtensions
{
    #region Fields

    private static readonly TokenType[] Factors = [TokenType.Multiply, TokenType.Divided];

    private static readonly TokenType[] Terms = [TokenType.Plus, TokenType.Minus];

    #endregion

    #region Methods

    public static SyntaxNode? BuildFactor(this ParsingContext context) =>
        context.Cursor.Peek().Type switch
        {
            TokenType.Multiply or TokenType.Divided => new FactorExpressionParser().Parse(context),
            _                                       => null
        };

    public static SyntaxNode? BuildTerm(this ParsingContext context) =>
        context.Cursor.Peek().Type switch
        {
            TokenType.Plus or TokenType.Minus => new TermExpressionParser().Parse(context),
            _                                 => null
        };

    public static bool IsFactor(this Token? token) => token is not null && Terms.Contains(token.Type);

    public static bool IsNumeric(this Token? token) => token is not null && token.Type == TokenType.Numeric;

    public static bool IsTerm(this Token? token) => token is not null && Terms.Contains(token.Type);

    #endregion
}