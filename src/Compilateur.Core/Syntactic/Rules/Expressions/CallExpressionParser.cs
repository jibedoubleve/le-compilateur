using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class CallExpressionParser : PrecedenceParser<PrimaryExpressionParser>
{
    #region Methods

    private IEnumerable<SyntaxNode> GetSubNodes(ParsingContext context)
    {
        var children = new List<SyntaxNode>
        {
            new(context.Cursor.Consume())
        };

        context.Cursor.Consume(); // Drop the '('

        while (!context.Cursor.IsAtEnd && context.Cursor.Peek().Type != TokenType.CloseParenthesis)
        {
            if (context.Cursor.Peek().Type == TokenType.Comma)
            {
                context.Cursor.Consume();
            }

            var expression = new ExpressionParser();
            if (!expression.Matches(context)) { break; }

            var node = expression.Parse(context);
            if (node is not null)
            {
                children.Add(node);
            }
        }

        context.Cursor.Consume(); // Drop the ')'
        return children;
    }

    protected override bool MatchesCurrent(ParsingContext context) => true;

    public override SyntaxNode? Parse(ParsingContext context)
    {
        var next = context.Cursor.PeekNext();
        if (next is null)
        {
            context.AddError("Unexpected end of input.");
            return null;
        }

        return next.Type == TokenType.OpenParenthesis
            ? new SyntaxNode(next, GetSubNodes(context))
            : InnerExpression.Parse(context);
    }

    #endregion
}