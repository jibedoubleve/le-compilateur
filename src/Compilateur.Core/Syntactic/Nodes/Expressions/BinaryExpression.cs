using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("binary")]

public record BinaryExpression : ExpressionNode
{
    #region Constructors

    public BinaryExpression(Token token, ExpressionNode left, ExpressionNode right) : base(token)
    {
        Left = left;
        Right = right;
    }

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Left, Right];

    public ExpressionNode Left { get; }

    public ExpressionNode Right { get; }

    #endregion
}