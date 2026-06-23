using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class FactorExpressionParser : PrecedenceParser<UnaryExpressionParser>
{
    #region Methods

    private SyntaxNode? Parse(ParsingContext context, SyntaxNode? accumulator)
    {
        if (!Matches(context)) { return accumulator; }
        
        return Parse(context,
            new SyntaxNode(
                context.Cursor.Consume(),
                accumulator,
                InnerExpression.Parse(context)
            ));
    }

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();
        return token.Type switch
        {
            TokenType.Multiply => true,
            TokenType.Divided  => true,
            _                  => false
        };
    }

    public override SyntaxNode? Parse(ParsingContext context)
        => Parse(
            context,
            InnerExpression.Parse(context)
        );

    #endregion
}