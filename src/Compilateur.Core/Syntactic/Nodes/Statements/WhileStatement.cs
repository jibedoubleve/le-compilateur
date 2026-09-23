using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("while")]
public sealed class WhileStatement : StatementNode
{
    #region Constructors

    public WhileStatement(Token token, ExpressionNode? condition, StatementNode body) : base(token)
    {
        Condition = condition;
        Body = body;
    }

    #endregion

    #region Properties

    public StatementNode Body { get; }

    public ExpressionNode? Condition { get; }

    #endregion
}