using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Declarations;

internal class VarDeclarationParser : Parser<VarDeclarationStatement>
{
    #region Fields

    private readonly ExpressionParser _expressionParser = new();

    #endregion

    #region Methods

    protected override VarDeclarationStatement? ParseCore(ParsingContext context)
    {
        context.Cursor.Consume(); // Consume the 'var'
        var varName = context.Cursor.Peek();

        if (varName.Kind != TokenKind.Identifier)
        {
            context.AddError($"Expected an identifier after 'var', found '{varName.Lexeme}'.");
            return null;
        }

        context.Cursor.Consume(); // consume the identifier
        if (context.Cursor.Peek().Kind != TokenKind.Assignment)
        {
            return context.ValidateFinalSemicolon(
                new VarDeclarationStatement(varName)
            );
        }

        // Handle initialisation...
        context.Cursor.Consume(); // Consume the '='

        if (!_expressionParser.Matches(context))
        {
            context.AddError($"Expected an expression, found {context.Cursor.Peek().Lexeme}.");
            return null;
        }
        var children = _expressionParser.Parse(context);
        if (children is null) { return null; }

        return context.ValidateFinalSemicolon(
            new VarDeclarationStatement(varName, children)
        );
    }

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Var);

    #endregion
}