using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("block")]
public sealed partial class BlockStatement(Token token, IEnumerable<StatementNode> statements) : StatementNode(token)
{
    #region Properties

    public IReadOnlyList<StatementNode> Statements { get; } = [.. statements];

    #endregion
}