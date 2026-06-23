using System.Runtime.InteropServices;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Declarations;

namespace Compilateur.Core.Syntactic.Helpers;

public static class BlockParser
{
    #region Fields

    private static readonly DeclarationParser DeclarationParser = new();

    #endregion

    #region Methods

    public static SyntaxNode? Parse(ParsingContext context)
    {
        var firstToken = context.Cursor.Peek();
        if (context.Cursor.IsPeekOfType(TokenType.OpenCurlyBracket))
        {
            context.Cursor.Consume(); // Consume the '{'
            var expressions = new List<SyntaxNode>();

            while (true)
            {
                if (context.Cursor.IsPeekOfType(TokenType.CloseCurlyBracket))
                {
                    context.Cursor.Consume();
                    break;
                }

                var node = DeclarationParser.Parse(context);
                if (node == null) { return null; }

                expressions.Add(SyntaxNode.Declaration(node));
            }

            return SyntaxNode.Unspecified(firstToken, [.. expressions]);
        }

        context.AddError("Expected '{' before block body.");
        return null;
    }

    #endregion
}