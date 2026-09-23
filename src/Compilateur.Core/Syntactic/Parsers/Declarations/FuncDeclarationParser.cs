using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Syntactic.Parsers.Declarations;

internal class FuncDeclarationParser : Parser<FunctionDeclarationStatement>
{
    #region Methods

    protected override FunctionDeclarationStatement? ParseCore(ParsingContext context)
    {
        context.Cursor.Consume(); // Consume the 'fun'

        return FunctionParser.Parse(context);
    }

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Fun);

    #endregion
}