using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public interface ISyntaxNodeVisitor
{
    #region Methods

    void Visit(SyntaxNode visitor);

    #endregion
}