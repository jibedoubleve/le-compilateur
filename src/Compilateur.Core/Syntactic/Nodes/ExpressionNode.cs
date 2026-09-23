using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class ExpressionNode : SyntaxNode
{
    #region Constructors

    protected ExpressionNode(Token token) : base(token) { }

    #endregion
}