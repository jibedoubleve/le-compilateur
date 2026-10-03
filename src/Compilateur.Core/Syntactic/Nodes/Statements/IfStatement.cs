using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("id")]
public sealed partial class IfStatement : StatementNode
{
    #region Constructors

    public IfStatement(
        Token token,
        ExpressionNode condition,
        StatementNode thenBranch,
        StatementNode? elseBranch = null) : base(token)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    #endregion

    #region Properties

    public ExpressionNode Condition { get; }
    public StatementNode? ElseBranch { get; }
    public StatementNode ThenBranch { get; }

    #endregion
}