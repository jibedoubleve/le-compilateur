using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("id")]
public sealed partial class IfStatement(
    Token token,
    ExpressionNode condition,
    StatementNode thenBranch,
    StatementNode? elseBranch = null)
    : StatementNode(token)
{
    #region Properties

    public ExpressionNode Condition { get; } = condition;
    public StatementNode? ElseBranch { get; } = elseBranch;
    public StatementNode ThenBranch { get; } = thenBranch;

    #endregion
}