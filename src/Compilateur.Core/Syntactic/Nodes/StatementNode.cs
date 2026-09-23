using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract record StatementNode : SyntaxNode
{
    #region Constructors

    protected StatementNode(Token token) : base(token) { }

    #endregion
}