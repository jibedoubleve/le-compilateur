using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public sealed partial class ProgramNode(Token token, IEnumerable<StatementNode> statements) : SyntaxNode(token)
{
    #region Properties

    public IReadOnlyList<StatementNode> Statements { get; } = [.. statements];

    #endregion
}