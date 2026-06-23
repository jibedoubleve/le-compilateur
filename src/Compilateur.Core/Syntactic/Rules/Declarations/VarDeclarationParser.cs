using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

internal class VarDeclarationParser : IParser
{
    #region Fields

    private readonly ExpressionParser _expressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Var);

    public SyntaxNode? Parse(ParsingContext context)
    {
        context.Cursor.Consume(); // Consume the 'var'
        var current = context.Cursor.Consume(); // consume the identifier
        if (current.Type != TokenType.Identifier)
        {
            context.AddError("Expected an identifier after 'var'");
            return null;
        }

        if (context.Cursor.Peek().Type != TokenType.Assignment)
        {
            return SyntaxNode.Unspecified(current);
        }

        // Handle assignment...
        context.Cursor.Consume(); // Consume the '='

        var children = _expressionParser.Parse(context);
        if (children is null)
        {
            context.AddError("Invalid assignment expression");
            return null;
        }

        return SyntaxNode.Var(current, children);
    }

    #endregion
}