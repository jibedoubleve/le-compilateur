namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed partial class ExpressionStatement(ExpressionNode expression) : StatementNode(expression.Token)
{
    #region Properties

    public ExpressionNode Expression { get; } = expression;

    #endregion
}