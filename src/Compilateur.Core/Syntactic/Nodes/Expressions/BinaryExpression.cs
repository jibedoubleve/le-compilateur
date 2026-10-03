using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("binary")]
public sealed partial class BinaryExpression : ExpressionNode
{
    #region Constructors

    public BinaryExpression(Token token, ExpressionNode left, ExpressionNode right) : base(token)
    {
        Left = left;
        Right = right;
    }

    #endregion

    #region Properties

    public ExpressionNode Left { get; }

    public ExpressionNode Right { get; }

    #endregion
}