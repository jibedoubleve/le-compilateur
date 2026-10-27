using Compilateur.Core.Syntactic.Nodes;

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
            context.AddError("Expect code, nothing to parse.");
            return null;
        }

        var children = new List<StatementNode>();
        while (!context.Cursor.IsAtEnd)
        {
            var node = DeclarationParser.Parse(context);
            if (node is null)
            {
                context.Cursor.Synchronise();
            }

            if (node is null) { continue; }

            children.Add(node);
        }

        return context.Errors.Any()
            ? null
            : BuildProgramNode();

        ProgramNode BuildProgramNode()
        {
            var current = context.Cursor.Peek();
            return new ProgramNode(current, [.. children]);
        }
    }

    #endregion
}