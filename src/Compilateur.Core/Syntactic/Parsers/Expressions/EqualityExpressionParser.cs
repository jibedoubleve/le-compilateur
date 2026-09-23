using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class EqualityExpressionParser : PrecedenceParser<ComparisonExpressionParser>
{
    #region Methods

    private ExpressionNode? Parse(ParsingContext context, ExpressionNode accumulator)
    {
        if (!MatchesCurrent(context)) { return accumulator; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);
        if (right is null) { return null; }

        return Parse(context,
            new BinaryExpression(
                operation,
                accumulator,
                right
            ));
    }

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();
        return token.Kind switch
        {
            TokenKind.Equality   => true,
            TokenKind.Inequality => true,
            _                    => false
        };
    }

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var accumulator = InnerExpression.Parse(context);
        return accumulator is null ? null : Parse(context, accumulator);
    }

    #endregion
}