using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class SyntaxNode(Token token)
{
    #region Properties

    public Token Token { get; } = token;

    #endregion

    #region Methods

    public abstract T Accept<T>(ISyntaxNodeVisitor<T> visitor);

    #endregion
}