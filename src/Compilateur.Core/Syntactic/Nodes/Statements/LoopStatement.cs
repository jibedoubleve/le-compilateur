using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

public record LoopStatement : StatementNode
{
    #region Constructors

    private LoopStatement(
        Token token, StatementNode? initialiser, ExpressionNode? condition, ExpressionNode? increment,
        StatementNode body)
        : base(token)
    {
        Initialiser = initialiser;
        Condition = condition;
        Body = body;
        Increment = increment;
    }

    #endregion

    #region Properties

    public StatementNode Body { get; }

    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            if (Initialiser is not null) { yield return Initialiser; }

            if (Condition is not null) { yield return Condition; }

            if (Increment is not null) { yield return Increment; }

            yield return Body;
        }
    }

    public ExpressionNode? Condition { get; }
    public ExpressionNode? Increment { get; }
    public StatementNode? Initialiser { get; }

    #endregion

    #region Methods

    public static LoopStatement For(
        Token token, StatementNode? initialiser, ExpressionNode? condition, ExpressionNode? increment,
        StatementNode body) => new(token,
        initialiser,
        condition,
        increment,
        body);

    public static LoopStatement While(Token token, ExpressionNode? condition, StatementNode body)
        => new(token,
            null,
            condition,
            null,
            body);

    #endregion
}