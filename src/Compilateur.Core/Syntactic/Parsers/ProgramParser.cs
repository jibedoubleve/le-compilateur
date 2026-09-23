using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Parsers.Declarations;

namespace Compilateur.Core.Syntactic.Parsers;

public static class ProgramParser
{
    #region Fields

    private static readonly DeclarationParser DeclarationParser = new();

    #endregion

    #region Methods

    public static ProgramNode? Parse(ParsingContext context)
    {
        if (context.Cursor.IsEmpty)
        {
            context.AddError("Expected code, nothing to parse.");
            return null;
        }

        var children = new List<StatementNode>();
        while (!context.Cursor.IsAtEnd)
        {
            var node = DeclarationParser.Parse(context);
            if (node is null) { return null; }

            children.Add(node);
        }

        var current = context.Cursor.Peek();
        return new ProgramNode(current, [.. children]);
    }

    #endregion
}