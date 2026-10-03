using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("unary")]
public sealed partial class UnaryExpression : ExpressionNode
{
    #region Constructors

    public UnaryExpression(Token token, ExpressionNode operand) : base(token) => Operand = operand;

    #endregion

    #region Properties

    public ExpressionNode Operand { get; }

    #endregion
}