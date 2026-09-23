using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed class PrintStatement : StatementNode
{
    #region Constructors

    public PrintStatement(Token token, ExpressionNode value) : base(token) => Value = value;

    #endregion

    #region Properties

    public ExpressionNode Value { get; }

    #endregion
}