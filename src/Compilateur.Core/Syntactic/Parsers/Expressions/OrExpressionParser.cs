using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class OrExpressionParser : PrecedenceParser<AndExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Or);

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var left = InnerExpression.Parse(context);
        if (left == null) { return null; }

        while (MatchesCurrent(context))
        {
            var operation = context.Cursor.Consume();
            var right = InnerExpression.Parse(context);
            if (right is null) { return null; }

            left = new LogicalExpression(operation, left, right);
        }

        return left;
    }

    #endregion
}