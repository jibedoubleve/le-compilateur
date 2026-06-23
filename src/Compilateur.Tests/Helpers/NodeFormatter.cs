using System.Text;
using Compilateur.Core.Syntactic;

namespace Compilateur.Tests.Helpers;

internal static class SyntaxNodeExtensions
{
    #region Methods

    private static void FormatTree(
        this SyntaxNode node, StringBuilder stringBuilder, int tabs, string treePrefix = "")
    {
        stringBuilder.AppendLine(
            $"{Tabulations(tabs)}{treePrefix} {node.Token.Lexeme} [{node.Token.Type}]"
        );

        var max = node.Children.Count();
        for (var i = 0; i < max; i++)
        {
            node.Children
                .ElementAt(i)
                .FormatTree(stringBuilder, tabs + 1, GetPrefix(i, max));
        }
    }

    private static string GetPrefix(int i, int max) => i < max-1 ? " ├──" : " └──";

    private static string Tabulations(int tabs = 0)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < tabs; i++) builder.Append(" │  ");
        return builder.ToString();
    }

    public static string FormatTree(this SyntaxNode node)
    {
        var stringBuilder = new StringBuilder();
        node.FormatTree(stringBuilder, -1);
        return stringBuilder.ToString();
    }

    #endregion
}