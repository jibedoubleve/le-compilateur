using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("binary")]
public sealed partial class BinaryExpression(Token token, ExpressionNode left, ExpressionNode right)
    : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Left { get; } = left;

    public ExpressionNode Right { get; } = right;

    #endregion
}