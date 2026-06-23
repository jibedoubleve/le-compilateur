using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class AssignmentExpression : PrecedenceParser<OrExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context)
    {
        var current = context.Cursor.Peek();

        if (current.Type != TokenType.Var)
        {
            return current.Type == TokenType.Identifier;
        }

        var next = context.Cursor.PeekNext();
        return next?.Type == TokenType.Identifier;
    }

    public override SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError("Expected expression after assignment operator");
            return null;
        }

        if (context.Cursor.Peek().Type == TokenType.Var)
        {
            context.Cursor.Consume(); // drop the 'var' 
        }

        var left = InnerExpression.Parse(context);
        if (!IsTokenAssignment(context)) { return left; }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);

        return new SyntaxNode(operation, left, right);

    }

    private static bool IsTokenAssignment(ParsingContext context) 
        => context.Cursor.Peek().Type == TokenType.Assignment;

    #endregion
}