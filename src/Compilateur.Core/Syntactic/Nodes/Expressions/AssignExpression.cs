using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("assign")]
public record AssignExpression : ExpressionNode
{
    #region Constructors

    public AssignExpression(Token token, IdentifierExpression target, ExpressionNode value) : base(token)
    {
        Target = target;
        Value = value;
    }

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Target, Value];

    public IdentifierExpression Target { get; }
    public ExpressionNode Value { get; }

    #endregion
}