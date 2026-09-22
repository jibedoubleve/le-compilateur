using Compilateur.Core.Lexical.Tokens;

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
            TokenType.This            => true,
            TokenType.Super           => true,
            _                         => false
        };
    }

    public SyntaxNode? Parse(ParsingContext context)
    {
        var current = context.Cursor.Consume();

        /* Keep track of pending parenthesis. If it is the case, we
         * have to consume the closing ')'
         */
        var isParenthesisPending = current.IsOfType(TokenType.OpenParenthesis);
        var token = isParenthesisPending
            ? new ExpressionParser().Parse(context)
            : SyntaxNode.Unspecified(current);

        if (!isParenthesisPending) { return token; }

        if (!context.Cursor.IsPeekOfType(TokenType.CloseParenthesis))
        {
            context.AddError($"Expected ')' after expression but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        context.Cursor.Consume();
        return token;
    }

    #endregion
}