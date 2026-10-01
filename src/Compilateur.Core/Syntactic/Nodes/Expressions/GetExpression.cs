using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("get")]
public sealed partial class GetExpression(Token token, ExpressionNode o) : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Object { get; } = o;

    #endregion
}