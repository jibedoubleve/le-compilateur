using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class FactorExpressionParser : PrecedenceParser<UnaryExpressionParser>
{
    #region Methods

    private SyntaxNode? Parse(ParsingContext context, SyntaxNode? accumulator)
    {
        if (!MatchesCurrent(context)) { return accumulator; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);
        if (right == null)
        {
            context.AddError("Missing factor's right operand.");
            return null;
        }

        return Parse(
            context,
            SyntaxNode.Unspecified(operation, accumulator, right)
        );
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
        => Parse(context, InnerExpression.Parse(context));

    #endregion
}