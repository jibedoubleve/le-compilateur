using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class UnaryExpressionParser : PrecedenceParser<CallExpressionParser>
{
    #region Fields

    private static readonly TokenType[] Tokens = [TokenType.Minus, TokenType.Bang];

    #endregion

    #region Methods

    private SyntaxNode? ParseOperand(ParsingContext context)
    {
        if (!InnerExpression.Matches(context))
        {
            context.AddError("Expected expression after unary operator");
            return null;
        }

        return InnerExpression.Parse(context);
    }

    private SyntaxNode? ParseUnaryOperator(ParsingContext context)
    {
        var current = context.Cursor.Consume(); // drop the operator '!' or '-'

        var node = Tokens.Contains(current.Type)
            ? Parse(context)
            : ParseOperand(context);

        return node is not null
            ? new SyntaxNode(current, [node])
            : null;
    }


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
        var current = context.Cursor.Peek();
        return current.Type is TokenType.Bang or TokenType.Minus
            ? ParseUnaryOperator(context)
            : InnerExpression.Parse(context);
    }

    #endregion
}