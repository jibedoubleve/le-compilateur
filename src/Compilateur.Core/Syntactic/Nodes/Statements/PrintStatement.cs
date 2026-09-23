using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public record PrintStatement : StatementNode
{
    #region Constructors

    public PrintStatement(Token token, ExpressionNode value) : base(token) => Value = value;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Value];
    public ExpressionNode Value { get; }

    #endregion
}