using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class AndExpressionParser : PrecedenceParser<EqualityExpressionParser>
{
    #region Methods

    private SyntaxNode? Parse(ParsingContext context, SyntaxNode? accumulator)
    {
        if (!Matches(context)) { return accumulator; }

        return Parse(
            context,
            new SyntaxNode(
                context.Cursor.Consume(),
                accumulator,
                InnerExpression.Parse(context)
            ));
    }

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var node = context.Cursor.Peek();
        return node.Type == TokenType.And;
    }

    public override SyntaxNode? Parse(ParsingContext context) => Parse(context, InnerExpression.Parse(context));

    #endregion
}