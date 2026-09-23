using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("unary")]
public record UnaryExpression : ExpressionNode
{
    #region Constructors

    public UnaryExpression(Token token, ExpressionNode operand) : base(token) => Operand = operand;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Operand];
    public ExpressionNode Operand { get; }

    #endregion
}