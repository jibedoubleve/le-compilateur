using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public sealed class ReturnStatement : StatementNode
{
    #region Constructors

    public ReturnStatement(Token token, ExpressionNode? expression = null) : base(token) => Expression = expression;

    #endregion

    #region Properties

    public ExpressionNode? Expression { get; }

    #endregion
}