using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("call")]
public sealed partial class CallExpression(Token token, ExpressionNode callee, ExpressionNode[] arguments)
    : ExpressionNode(token)
{
    #region Properties

    public IReadOnlyList<ExpressionNode> Arguments { get; } = [.. arguments];
    public ExpressionNode Callee { get; } = callee;

    #endregion
}