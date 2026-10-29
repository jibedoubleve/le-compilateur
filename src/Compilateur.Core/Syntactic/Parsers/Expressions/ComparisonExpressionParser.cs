using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class ComparisonExpressionParser : PrecedenceParser<TermExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();
        return token.Kind switch
        {
            TokenKind.GreaterThan        => true,
            TokenKind.GreaterThanOrEqual => true,
            TokenKind.LessThan           => true,
            TokenKind.LessThanOrEqual    => true,
            _                            => false
        };
    }

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var left = InnerExpression.Parse(context);
        if (left is null) { return null; }

        while (MatchesCurrent(context))
        {
            var operation = context.Cursor.Consume();
            var right = InnerExpression.Parse(context);
            if (right is null) { return null; }

            left = new BinaryExpression(operation, left, right);
        }

        return left;
    }

    #endregion
}