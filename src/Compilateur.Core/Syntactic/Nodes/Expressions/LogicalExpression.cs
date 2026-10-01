using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("logical")]
public sealed partial class LogicalExpression(Token token, ExpressionNode left, ExpressionNode right)
    : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Left { get; } = left;

    public ExpressionNode Right { get; } = right;

    #endregion
}