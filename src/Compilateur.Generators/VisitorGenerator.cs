using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Compilateur.Generators;

[Generator]
public class VisitorGenerator : IIncrementalGenerator
{
    #region Methods

    private static void GetAcceptCode(SourceProductionContext context, NodeInfo? node)
    {
        if (node is null) { return; }

        var fileName = $"{node.Name}.Accept.g.cs";
        var code = $$"""
                     using System.Diagnostics;
                     using Compilateur.Core.Syntactic;

                     namespace {{node.Namespace}};

                     public partial class {{node.Name}}
                     {
                        [StackTraceHidden, DebuggerNonUserCode, DebuggerStepThrough] 
                        public override T Accept<T>(ISyntaxNodeVisitor<T> visitor) => visitor.Visit(this);
                     }
                     """;
        context.AddSource(fileName, code);
    }

    private static void GetVisitorCode(SourceProductionContext context, ImmutableArray<NodeInfo?> nodes)
    {
        var overloads = new StringBuilder();
        const string fileName = "ISyntaxNodeVisitor.g.cs";

        foreach (var type in nodes.Where(n => n is not null)
                                  .Select(node => $"{node!.Namespace}.{node.Name}")
                )
        {
            overloads.AppendLine($"    T Visit({type} node);");
        }

        var code = $$"""
                     namespace Compilateur.Core.Syntactic;

                     public interface ISyntaxNodeVisitor<T>
                     {
                     {{overloads}}
                     }
                     """;

        context.AddSource(fileName, code);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
        => node is ClassDeclarationSyntax;

    private static NodeInfo? Transform(GeneratorSyntaxContext ctx, CancellationToken token)
    {
        var symbolNode = ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, token);
        if (symbolNode is null ||
            symbolNode.IsAbstract ||
            symbolNode is not INamedTypeSymbol symbol)
        {
            return null;
        }

        if (!IsSyntaxNode(symbol)) { return null; }

        return new NodeInfo(
            symbol.Name,
            symbol.ContainingNamespace.ToDisplayString()
        );

        bool IsSyntaxNode(INamedTypeSymbol node)
        {
            const string nodeBaseType = "Compilateur.Core.Syntactic.Nodes.SyntaxNode";
            if (node.BaseType is null) { return false; }

            return node.BaseType.ToDisplayString().Equals(nodeBaseType, StringComparison.InvariantCulture)
                   || IsSyntaxNode(node.BaseType);
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transform)
                              .Where(x => x is not null);
        context.RegisterSourceOutput(provider, GetAcceptCode);
        context.RegisterSourceOutput(provider.Collect(), GetVisitorCode);
    }

    #endregion

    private sealed record NodeInfo(string Name, string Namespace)
    {
        #region Properties

        public string Name { get; } = Name;
        public string Namespace { get; } = Namespace;

        #endregion
    }
}