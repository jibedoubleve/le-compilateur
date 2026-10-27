using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Parsers;

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
            var hasCurlyBrace = false;
            while (!context.Cursor.IsAtEnd)
            {
                if (context.Cursor.IsPeekOfKind(TokenKind.CloseCurlyBracket))
                {
                    hasCurlyBrace = true;
                    context.Cursor.Consume();
                    break;
                }

                if (!DeclarationParser.Matches(context))
                {
                    context.AddError($"Expect a declaration or a statement, found {context.Cursor.Peek().Lexeme}.");
                    context.Cursor.Synchronise();
                }

                var node = DeclarationParser.Parse(context);
                if (node is null)
                {
                    context.Cursor.Synchronise();
                }
                else
                {
                    children.Add(node);
                }
            }

            if (!hasCurlyBrace) { context.AddError("Expect '}' after block body."); }

            return new BlockStatement(firstToken, [.. children]);
        }

        context.AddError("Expect '{' before block body.");
        return null;
    }

    #endregion
}