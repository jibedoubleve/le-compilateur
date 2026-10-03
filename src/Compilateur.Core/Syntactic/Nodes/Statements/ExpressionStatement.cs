namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed partial class ExpressionStatement : StatementNode
{
    #region Constructors

    public ExpressionStatement(ExpressionNode expression) : base(expression.Token) => Expression = expression;

    #endregion

    #region Properties

    public ExpressionNode Expression { get; }

    #endregion
}