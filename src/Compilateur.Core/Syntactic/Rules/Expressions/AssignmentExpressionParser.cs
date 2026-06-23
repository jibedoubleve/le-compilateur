using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class AssignmentExpressionParser : PrecedenceParser<OrExpressionParser>
{
    #region Methods

    protected override bool MatchesCurrent(ParsingContext context) 
        => context.Cursor.IsPeekOfType(TokenType.Identifier)
           && context.Cursor.IsPeekNextOfType(TokenType.Assignment);

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

        if (!context.Cursor.IsPeekOfType(TokenType.Assignment)) { return left; }

        if (!IsValidLeftOperand(left))
        {
            context.AddError(
                $"Expected an identifier or a property access on the left-hand side " +
                $"of an assignment, but found '{left?.Token.Lexeme ?? "nothing"}'.");
            return null;
        }

        var operation = context.Cursor.Consume();
        var right = InnerExpression.Parse(context);

        return SyntaxNode.Unspecified(operation, left, right);

        bool IsValidLeftOperand(SyntaxNode? node) =>
            node.IsOfType(TokenType.Identifier)
            && node.IsOneOfRole(SyntaxNodeRole.Unspecified, SyntaxNodeRole.Get);
    }

    #endregion
}