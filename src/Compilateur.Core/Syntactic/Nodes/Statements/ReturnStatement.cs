using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed partial class ReturnStatement(Token token, ExpressionNode? expression = null) : StatementNode(token)
{
    #region Properties

    public ExpressionNode? Expression { get; } = expression;

    #endregion
}