using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("assign")]
public sealed partial class AssignExpression : ExpressionNode
{
    #region Constructors

    public AssignExpression(Token token, IdentifierExpression target, ExpressionNode value) : base(token)
    {
        Target = target;
        Value = value;
    }

    #endregion

    #region Properties

    public IdentifierExpression Target { get; }
    public ExpressionNode Value { get; }

    #endregion
}