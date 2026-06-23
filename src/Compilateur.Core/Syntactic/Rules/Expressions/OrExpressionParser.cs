using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class OrExpressionParser : PrecedenceParser<AndExpressionParser>
{
    #region Methods

    private SyntaxNode? Parse(ParsingContext context, SyntaxNode? accumulator)
    {
        if (!MatchesCurrent(context)) { return accumulator; }

        return Parse(
            context,
            SyntaxNode.Unspecified(
                context.Cursor.Consume(),
                accumulator,
                InnerExpression.Parse(context)
            ));
    }

    protected override bool MatchesCurrent(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Or);
    public override SyntaxNode? Parse(ParsingContext context) => Parse(context, InnerExpression.Parse(context));

    #endregion
}