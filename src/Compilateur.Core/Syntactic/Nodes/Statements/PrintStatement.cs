using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed partial class PrintStatement(Token token, ExpressionNode value) : StatementNode(token)
{
    #region Properties

    public ExpressionNode Value { get; } = value;

    #endregion
}