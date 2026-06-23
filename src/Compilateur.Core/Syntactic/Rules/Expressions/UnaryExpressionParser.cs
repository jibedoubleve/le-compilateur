using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class UnaryExpressionParser : PrecedenceParser<CallExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();

        return token.Type switch
        {
            TokenType.Minus => true,
            TokenType.Bang  => true,
            _               => false
        };
    }

    public override SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError("Expected expression after unary operator");
            return null;
        }

        if (!MatchesCurrent(context)) { return InnerExpression.Parse(context); }

        var operation = context.Cursor.Consume();
        var child = Parse(context);

        return child is null
            ? null
            : new SyntaxNode(operation, child);
    }

    #endregion
}