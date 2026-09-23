using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class PrimaryExpressionParser : IParser<ExpressionNode>
{
    #region Methods

    private static ExpressionNode? ParseAtom(Token token, ParsingContext context)
    {
        switch (token.Kind)
        {
            case TokenKind.Identifier: return new IdentifierExpression(token);
            case TokenKind.This: return new ThisExpression(token);
            case TokenKind.Super: return ParseSuper(token, context);
            case TokenKind.Numeric:
            case TokenKind.String:
            case TokenKind.True:
            case TokenKind.False:
            case TokenKind.Nil:
                return new LiteralExpression(token);
            default:
                context.AddError($"Expect expression, found '{token.Lexeme}'.");
                return null;
        }
    }

    private static ExpressionNode? ParseSuper(Token token, ParsingContext context)
    {
        if (!context.Cursor.IsPeekOfKind(TokenKind.Dot))
        {
            context.AddError($"Expect '.' after 'super', found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        context.Cursor.Consume(); // consume the "."

        if (!context.Cursor.IsPeekOfKind(TokenKind.Identifier))
        {
            context.AddError($"Expect superclass method name, found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        return new SuperExpression(token, context.Cursor.Consume());
    }

    public bool Matches(ParsingContext context)
    {
        var token = context.Cursor.Peek();

        return token.Kind switch
        {
            TokenKind.OpenParenthesis => true,
            TokenKind.Identifier      => true,
            TokenKind.Numeric         => true,
            TokenKind.String          => true,
            TokenKind.True            => true,
            TokenKind.False           => true,
            TokenKind.Nil             => true,
            TokenKind.This            => true,
            TokenKind.Super           => true,
            _                         => false
        };
    }

    public ExpressionNode? Parse(ParsingContext context)
    {
        var current = context.Cursor.Consume();

        /* Keep track of pending parenthesis. If it is the case, we
         * have to consume the closing ')'
         */
        var isParenthesisPending = current.IsOfKind(TokenKind.OpenParenthesis);
        var expression = isParenthesisPending
            ? new ExpressionParser().Parse(context)
            : ParseAtom(current, context);

        if (expression is null) { return null; }

        if (!isParenthesisPending) { return expression; }

        if (!context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis))
        {
            context.AddError($"Expected ')' after expression but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        context.Cursor.Consume();
        return expression;
    }

    #endregion
}