using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("id")]
public record IfStatement : StatementNode
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

    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            yield return Condition;
            yield return ThenBranch;
            if (ElseBranch is not null)
            {
                yield return ElseBranch;
            }
        }
    }

    public ExpressionNode Condition { get; }
    public StatementNode? ElseBranch { get; }
    public StatementNode ThenBranch { get; }

    #endregion
}