using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Statements;

[Description("block")]
public record BlockStatement : StatementNode
{
    #region Constructors

    public BlockStatement(Token token, IEnumerable<StatementNode> statements) : base(token) => Statements = statements;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => Statements;

    public IEnumerable<StatementNode> Statements { get; }

    #endregion
}