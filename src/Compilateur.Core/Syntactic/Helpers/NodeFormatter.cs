using System.ComponentModel;
using System.Reflection;
using System.Text;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic.Helpers;

internal static class SyntaxNodeExtensions
{
    #region Fields

    private static readonly HashSet<int> ClosedNodes = new();

    #endregion

    #region Methods

    private static string Describe(SyntaxNode node)
    {
        string?[] strings =
        [
            node.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description,
            GetTokenKind(node)
        ];

        var ret = string.Join(", ", strings.Where(x => !string.IsNullOrEmpty(x)));
        return string.IsNullOrEmpty(ret) ? string.Empty : $"[{ret}]";
    }

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
            $"{FormatHeader(depth)}{treeNode} {node.Token.Lexeme} {Describe(node)}"
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

    private static string GetTokenKind(SyntaxNode node) =>
        node.Token.Kind switch
        {
            TokenKind.Numeric => $"{node.Token.Kind}",
            TokenKind.String  => $"{node.Token.Kind}",
            TokenKind.Eof     => "EOF",
            _                 => string.Empty
        };

    public static string FormatTree(this SyntaxNode node)
    {
        ClosedNodes.Clear();
        var stringBuilder = new StringBuilder();
        node.FormatTree(stringBuilder, -1);
        return stringBuilder.ToString();
    }

    #endregion
}