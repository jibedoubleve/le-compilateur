using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class AssignmentExpressionParser : PrecedenceParser<OrExpressionParser>
{
    #region Methods

    private static bool IsTokenAssignment(ParsingContext context)
        => context.Cursor.IsPeekOfType(TokenType.Assignment);

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var current = context.Cursor.Peek();

        if (current.Type != TokenType.Var)
        {
            return current.IsOfType(TokenType.Identifier);
        }

        var next = context.Cursor.PeekNext();
        return next?.IsOfType(TokenType.Identifier) ?? false;
    }

    public override SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError("Expected expression after assignment operator");
            return null;
        }

        if (context.Cursor.IsPeekOfType(TokenType.Var))
        {
            context.Cursor.Consume(); // drop the 'var' 
        }

        var left = InnerExpression.Parse(context);
        if (!IsTokenAssignment(context)) { return left; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);

        return SyntaxNode.Unspecified(operation, left, right);
    }

    #endregion
}