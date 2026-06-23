using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class CallExpressionParser : PrecedenceParser<PrimaryExpressionParser>
{
    #region Methods

    private static SyntaxNode[] GetSubNodes(ParsingContext context)
    {
        var children = new List<SyntaxNode>();

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
        if(!context.Cursor.IsAtEnd)
        {
            context.Cursor.Consume(); // Drop the ')'
        }
        return [.. children];
    }

    protected override bool MatchesCurrent(ParsingContext context) => false;

    public override SyntaxNode? Parse(ParsingContext context)
    {
        var node = InnerExpression.Parse(context);
        var current = context.Cursor.Peek();
        if (node is not null && current.Type == TokenType.OpenParenthesis)
        {
            context.Cursor.Consume();
            return new SyntaxNode(
                node.Token,
                GetSubNodes(context)
            );
        }

        return node;
    }

    #endregion
}