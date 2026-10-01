using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("while")]
public sealed partial class WhileStatement(Token token, ExpressionNode? condition, StatementNode body)
    : StatementNode(token)
{
    #region Properties

    public StatementNode Body { get; } = body;

    public ExpressionNode? Condition { get; } = condition;

    #endregion
}