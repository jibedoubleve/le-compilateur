using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("unary")]
public sealed partial class UnaryExpression(Token token, ExpressionNode operand) : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Operand { get; } = operand;

    #endregion
}