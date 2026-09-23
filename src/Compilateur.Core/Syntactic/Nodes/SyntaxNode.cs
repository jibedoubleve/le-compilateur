using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract record SyntaxNode
{
    #region Constructors

    protected SyntaxNode(Token token) => Token = token;

    #endregion

    #region Properties

    public virtual IEnumerable<SyntaxNode> Children { get; } = [];

    public Token Token { get; }

    #endregion
}