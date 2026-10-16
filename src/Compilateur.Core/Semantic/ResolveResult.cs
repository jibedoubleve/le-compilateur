using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public sealed record ResolveResult(ErrorCollection Errors, IDictionary<SyntaxNode, uint> DistanceMap)
{
    public bool HasErrors => Errors.Any();
}