using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class OrExpressionParser : PrecedenceParser<AndExpressionParser>
{
    #region Methods

    private ExpressionNode? Parse(ParsingContext context, ExpressionNode accumulator)
    {
        if (!MatchesCurrent(context)) { return accumulator; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);
        if (right is null) { return null; }

        return Parse(
            context,
            new LogicalExpression(
                operation,
                accumulator,
                right
            ));
    }

    protected override bool MatchesCurrent(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Or);

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var accumulator = InnerExpression.Parse(context);
        return accumulator is null ? null : Parse(context, accumulator);
    }

    #endregion
}