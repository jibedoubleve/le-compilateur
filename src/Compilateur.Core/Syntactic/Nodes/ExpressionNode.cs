using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract record ExpressionNode : SyntaxNode
{
    #region Constructors

    protected ExpressionNode(Token token) : base(token) { }

    #endregion
}