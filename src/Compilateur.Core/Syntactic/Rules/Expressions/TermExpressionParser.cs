using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class TermExpressionParser : PrecedenceParser<FactorExpressionParser>
{
    #region Methods

    private SyntaxNode? Parse(ParsingContext context, SyntaxNode? accumulator)
    {
        if (!MatchesCurrent(context)) { return accumulator; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);
        if (right == null)
        {
            context.AddError("Missing term's right operand.");
            return null;
        }

        var parsed = Parse(
            context,
            SyntaxNode.Unspecified(operation, accumulator, right)
        );
        return parsed;
    }

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();
        return token.Type switch
        {
            TokenType.Plus  => true,
            TokenType.Minus => true,
            _               => false
        };
    }

    public override SyntaxNode? Parse(ParsingContext context)
        => Parse(context, InnerExpression.Parse(context));

    #endregion
}