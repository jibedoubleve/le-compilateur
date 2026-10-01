using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("set")]
public sealed partial class SetExpression(Token token, ExpressionNode o, ExpressionNode value) : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Object { get; } = o;
    public ExpressionNode Value { get; } = value;

    #endregion
}