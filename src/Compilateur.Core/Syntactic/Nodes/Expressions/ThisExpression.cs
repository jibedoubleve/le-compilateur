using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("this")]
public record ThisExpression : ExpressionNode
{
    #region Constructors

    public ThisExpression(Token token) : base(token) { }

    #endregion
}