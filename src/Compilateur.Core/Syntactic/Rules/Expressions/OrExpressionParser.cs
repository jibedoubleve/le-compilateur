using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class OrExpressionParser : PrecedenceParser<AndExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context) => context.Cursor.Peek().Type == TokenType.Or;

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
    public override SyntaxNode? Parse(ParsingContext context) => Parse(context, InnerExpression.Parse(context));

    #endregion
}