using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public sealed partial class ProgramNode : SyntaxNode
{
    #region Constructors

    public ProgramNode(Token token, IEnumerable<StatementNode> statements) : base(token) =>
        Statements = [.. statements];

    #endregion

    #region Properties

    public IReadOnlyList<StatementNode> Statements { get; }

    #endregion
}