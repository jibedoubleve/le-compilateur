using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

public class VarDeclarationParser : IParser
{
    #region Fields

    private readonly ExpressionParser _expressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.Peek().Type == TokenType.Var;

    public SyntaxNode? Parse(ParsingContext context)
    {
        context.Cursor.Consume(); // Consume the 'var'
        var current = context.Cursor.Consume(); // consume the identifier
        if (current.Type != TokenType.Identifier)
        {
            context.AddError("Expected an identifier after 'var'");
            return null;
        }

        var next = context.Cursor.Peek();
        if (next.Type != TokenType.Assignment)
        {
            return new SyntaxNode(current);
        }

        // Handle assignment...
        context.Cursor.Consume(); // Consume the '='

        var children = _expressionParser.Parse(context);
        if (children is null)
        {
            context.AddError("Invalid assignment expression");
            return null;
        }

        return new SyntaxNode(current, children);

    }

    #endregion
}