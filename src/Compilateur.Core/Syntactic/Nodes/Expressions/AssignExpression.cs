using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("assign")]
public sealed partial class AssignExpression(Token token, IdentifierExpression target, ExpressionNode value)
    : ExpressionNode(token)
{
    #region Properties

    public IdentifierExpression Target { get; } = target;
    public ExpressionNode Value { get; } = value;

    #endregion
}