using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

internal class FuncDeclarationParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Fun);

    public SyntaxNode? Parse(ParsingContext context)
    {
        context.Cursor.Consume(); // Consume the 'fun'

        return FunctionParser.Parse(context);
    }

    #endregion
}