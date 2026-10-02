using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Semantic;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class SyntaxNode
{
    #region Constructors

    protected SyntaxNode(Token token) => Token = token;

    #endregion

    #region Properties

    public Token Token { get; }

    #endregion

    #region Methods

    public abstract void Accept(ISyntaxNodeVisitor visitor);

    #endregion
}