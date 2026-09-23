using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class UnaryExpressionParser : PrecedenceParser<CallExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var token = context.Cursor.Peek();

        return token.Kind switch
        {
            TokenKind.Minus => true,
            TokenKind.Bang  => true,
            _               => false
        };
    }

    public override ExpressionNode? Parse(ParsingContext context)
    {
        if (!MatchesCurrent(context)) { return InnerExpression.Parse(context); }

        var operation = context.Cursor.Consume();
        var child = Parse(context);

        return child is null
            ? null
            : new UnaryExpression(operation, child);
    }

    #endregion
}