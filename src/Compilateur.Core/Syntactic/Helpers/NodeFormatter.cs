using System.Text;
using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Syntactic.Helpers;

internal static class SyntaxNodeExtensions
{
    #region Fields

    private static readonly HashSet<int> ClosedNodes = new();

    #endregion

    #region Methods

    private static string FormatHeader(int depth = 0)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < depth; i++)
        {
            builder.Append(
                ClosedNodes.Contains(i)
                    ? "    "
                    : " │  "
            );
        }

        return builder.ToString();
    }

    private static void FormatTree(
        this SyntaxNode node, StringBuilder stringBuilder, int depth, string treeNode = "")
    {
        stringBuilder.AppendLine(
            $"{FormatHeader(depth)}{treeNode} {node.Token.Lexeme} [{FormatType(node)}]"
        );

        var max = node.Children.Count();
        for (var i = 0; i < max; i++)
        {
            node.Children
                .ElementAt(i)
                .FormatTree(
                    stringBuilder,
                    depth + 1,
                    FormatTreeNode(i, max, depth)
                );
        }
    }

    private static string FormatTreeNode(int i, int max, int depth)
    {
        if (i < max - 1) { return " ├──"; }

        ClosedNodes.Add(depth + 1);
        return " └──";
    }

    private static string FormatType(SyntaxNode node)
    {
        if (node.Role == SyntaxNodeRole.Unspecified)
        {
            return $"{node.Token.Type}";
        }

        return node.IsOfType(TokenType.Identifier) && node.Role != SyntaxNodeRole.Unspecified
            ? $"{node.Role}"
            : $"{node.Token.Type}, {node.Role}";
    }

    public static string FormatTree(this SyntaxNode node)
    {
        ClosedNodes.Clear();
        var stringBuilder = new StringBuilder();
        node.FormatTree(stringBuilder, -1);
        return stringBuilder.ToString();
    }

    #endregion
}