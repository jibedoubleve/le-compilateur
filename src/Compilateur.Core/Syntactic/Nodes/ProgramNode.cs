using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public record ProgramNode : SyntaxNode
{
    #region Constructors

    public ProgramNode(Token token, IEnumerable<StatementNode> statements) : base(token) =>
        Statements = statements;

    #endregion

    #region Properties

    public override IEnumerable<StatementNode> Children => Statements;
    public IEnumerable<StatementNode> Statements { get; }

    #endregion
}