using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Semantic;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class SyntaxNode(Token token)
{
    #region Properties

    public Token Token { get; } = token;

    #endregion

    #region Methods

    public abstract void Accept(ISyntaxNodeVisitor visitor);

    #endregion
}