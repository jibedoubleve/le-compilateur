using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("block")]
public sealed class BlockStatement : StatementNode
{
    #region Constructors

    public BlockStatement(Token token, IEnumerable<StatementNode> statements) : base(token) =>
        Statements = [.. statements];

    #endregion

    #region Properties

    public IReadOnlyList<StatementNode> Statements { get; }

    #endregion
}