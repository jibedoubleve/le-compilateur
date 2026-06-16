using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

public class VarDeclarationParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.Peek().Type == TokenType.Var;

    public SyntaxNode? Parse(ParsingContext context)
    {
        context.Cursor.Consume();

        var current = context.Cursor.Consume();
        if (current.Type != TokenType.Identifier)
        {
            context.AddError("Expected variable name after 'var'");
            return null;
        }

        return new SyntaxNode(current);
    }

    #endregion
}