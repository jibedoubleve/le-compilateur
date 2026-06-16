using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class PrimaryExpressionParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context)
    {
        var token = context.Cursor.Peek();

        return token.Type switch
        {
            TokenType.OpenParenthesis => true,
            TokenType.Identifier      => true,
            TokenType.Numeric         => true,
            TokenType.String          => true,
            TokenType.True            => true,
            TokenType.False           => true,
            TokenType.Nil             => true,
            _                         => false
        };
    }

    public SyntaxNode? Parse(ParsingContext context)
    {
        var current = context.Cursor.Consume();

        /* Keep track we are in pending parenthesis. If it is the case, we
         * have to consume the closing )
         */
        var isParenthesisPending = current.Type == TokenType.OpenParenthesis;
        var token = isParenthesisPending
            ? new ExpressionParser().Parse(context)
            : new SyntaxNode(current);

        if (isParenthesisPending 
            && context.Cursor.Peek().Type == TokenType.CloseParenthesis)
        {
            context.Cursor.Consume();
        }

        return token;
    }

    #endregion
}