using System.ComponentModel;
using System.Reflection;
using System.Text;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Helpers;

internal static class SyntaxNodeExtensions
{
    #region Methods

    private static IEnumerable<SyntaxNode> Children(this SyntaxNode node) =>
        node switch
        {
            BinaryExpression n                      => [n.Left, n.Right],
            VarDeclarationStatement n               => Optional(n.Initialiser),
            FunctionDeclarationStatement n          => [.. n.Parameters, n.Body],
            AssignExpression n                      => [n.Target, n.Value],
            ProgramNode n                           => n.Statements,
            CallExpression n                        => [n.Callee, .. n.Arguments],
            BlockStatement n                        => n.Statements,
            SetExpression n                         => [n.Object, n.Value],
            IfStatement { ElseBranch: { } @else } n => [n.Condition, n.ThenBranch, @else],
            IfStatement n                           => [n.Condition, n.ThenBranch],
            ClassStatement n                        => [.. Optional(n.SuperClass), .. n.Functions],
            GetExpression n                         => [n.Object],
            GroupExpressionNode n                   => [n.Inner],
            ReturnStatement n                       => Optional(n.Expression),
            PrintStatement n                        => [n.Value],
            UnaryExpression n                       => [n.Operand],
            ExpressionStatement n                   => [n.Expression],
            LogicalExpression n                     => [n.Left, n.Right],
            WhileStatement n                        => [.. Optional(n.Condition), n.Body],
            LiteralExpression
                or ParameterNode
                or IdentifierExpression
                or SuperExpression
                or ThisExpression => [],
            _ => throw new NotSupportedException($"Type {node.GetType()} not supported")
        };

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

    private static void FormatChildren(this SyntaxNode node, StringBuilder stringBuilder, string prefix)
    {
        var children = node.Children().ToList();
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var isLast = i == children.Count - 1;

            stringBuilder.AppendLine($"{prefix}{(isLast ? " └──" : " ├──")} {child.Token.Lexeme} {Describe(child)}");
            child.FormatChildren(stringBuilder, prefix + (isLast ? "    " : " │  "));
        }
    }

    private static string GetTokenKind(SyntaxNode node) =>
        node.Token.Kind switch
        {
            TokenKind.Numeric => $"{node.Token.Kind}",
            TokenKind.String  => $"{node.Token.Kind}",
            TokenKind.Eof     => "EOF",
            _                 => string.Empty
        };

    private static IEnumerable<SyntaxNode> Optional(SyntaxNode? node) => node is null ? [] : [node];

    public static string FormatTree(this SyntaxNode node)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($" {node.Token.Lexeme} {Describe(node)}");
        node.FormatChildren(stringBuilder, "");
        return stringBuilder.ToString();
    }

    #endregion
}