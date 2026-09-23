using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Expressions;

internal class AssignmentExpressionParser : PrecedenceParser<OrExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context)
        => context.Cursor.IsPeekOfKind(TokenKind.Identifier)
           && context.Cursor.IsPeekNextOfKind(TokenKind.Assignment);

    public override ExpressionNode? Parse(ParsingContext context)
    {
        var target = InnerExpression.Parse(context);
        if (target is null) { return null; }

        if (!context.Cursor.IsPeekOfKind(TokenKind.Assignment)) { return target; }

        var operation = context.Cursor.Consume();
        var value = Parse(context);
        if (value is null) { return null; }

        ExpressionNode? result = target switch
        {
            IdentifierExpression id => new AssignExpression(operation, id, value),
            GetExpression get       => new SetExpression(get.Token, get.Object, value),
            _                       => null
        };

        if (result is null) { context.AddError("Invalid assignment target."); }

        return result;
    }

    #endregion
}