using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class CallExpressionParser : PrecedenceParser<PrimaryExpressionParser>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    /// <remarks>
    ///     Expects the '(' to be already consumed. Consumes the closing ')'.
    ///     Returns null when an error has been reported.
    /// </remarks>
    private static ExpressionNode[]? GetArguments(ParsingContext context)
    {
        var arguments = new List<ExpressionNode>();

        if (!context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis))
        {
            while (true)
            {
                var argument = ExpressionParser.Parse(context);
                if (argument is null) { return null; }

                arguments.Add(argument);

                if (!context.Cursor.IsPeekOfKind(TokenKind.Comma)) { break; }

                context.Cursor.Consume(); // Consume the ','
            }
        }

        if (!context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis))
        {
            context.AddError($"Expect ')' after arguments, found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        context.Cursor.Consume(); // Consume the ')'
        return [.. arguments];
    }

    protected override bool MatchesCurrent(ParsingContext context) => false;

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var expr = InnerExpression.Parse(context);
        if (expr is null) { return null; }

        while (true)
        {
            if (context.Cursor.IsPeekOfKind(TokenKind.OpenParenthesis))
            {
                var parenthesis = context.Cursor.Consume();
                var arguments = GetArguments(context);
                if (arguments is null) { return null; }

                expr = new CallExpression(parenthesis, expr, arguments);
            }
            else if (context.Cursor.IsPeekOfKind(TokenKind.Dot))
            {
                context.Cursor.Consume(); // Consume the '.'

                if (!context.Cursor.IsPeekOfKind(TokenKind.Identifier))
                {
                    context.AddError($"Expect property name after '.', found '{context.Cursor.Peek().Lexeme}'.");
                    return null;
                }

                expr = new GetExpression(context.Cursor.Consume(), expr);
            }
            else
            {
                return expr;
            }
        }
    }

    #endregion
}