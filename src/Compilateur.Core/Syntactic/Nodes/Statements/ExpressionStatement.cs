namespace Compilateur.Core.Syntactic.Nodes.Statements;

public record ExpressionStatement : StatementNode
{
    #region Constructors

    public ExpressionStatement(ExpressionNode expression) : base(expression.Token) => Expression = expression;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Expression];
    public ExpressionNode Expression { get; }

    #endregion
}