using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public record ReturnStatement : StatementNode
{
    #region Constructors

    public ReturnStatement(Token token, ExpressionNode? expression = null) : base(token) => Expression = expression;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => Expression is null ? [] : [Expression];

    public ExpressionNode? Expression { get; }

    #endregion
}