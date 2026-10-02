using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public class Binder : ISyntaxNodeVisitor
{
    #region Fields

    public readonly Dictionary<SyntaxNode, uint> DistanceMap = new(ReferenceEqualityComparer.Instance);
    public readonly Stack<Scope> Scopes = new();

    #endregion

    #region Methods

    public void Visit(SyntaxNode visitor) => throw new NotImplementedException();

    #endregion
}