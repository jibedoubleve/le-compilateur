using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Compilateur.Core.Syntactic.Parsers;

namespace Compilateur.Core.Syntactic.Helpers;

public static class BlockParser
{
    #region Fields

    private static readonly DeclarationParser DeclarationParser = new();

    #endregion

    #region Methods

    public static BlockStatement? Parse(ParsingContext context)
    {
        var firstToken = context.Cursor.Peek();
        if (context.Cursor.IsPeekOfKind(TokenKind.OpenCurlyBracket))
        {
            context.Cursor.Consume(); // Consume the '{'

            var children = new List<StatementNode>();
            while (true)
            {
                if (context.Cursor.IsPeekOfKind(TokenKind.CloseCurlyBracket))
                {
                    context.Cursor.Consume();
                    break;
                }

                if (!DeclarationParser.Matches(context))
                {
                    context.AddError($"Expected a declaration or a statement, found {context.Cursor.Peek().Lexeme}.");
                    return null;
                }

                var node = DeclarationParser.Parse(context);
                if (node == null) { return null; }

                children.Add(node);
            }

            return new BlockStatement(firstToken, [.. children]);
        }

        context.AddError("Expected '{' before block body.");
        return null;
    }

    #endregion
}