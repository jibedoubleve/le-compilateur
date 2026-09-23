using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class SyntaxNode
{
    #region Constructors

    protected SyntaxNode(Token token) => Token = token;

    #endregion

    #region Properties

    public Token Token { get; }

    #endregion
}