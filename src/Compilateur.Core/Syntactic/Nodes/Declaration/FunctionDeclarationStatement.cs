using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("function")]
public sealed partial class FunctionDeclarationStatement(
    Token token,
    IEnumerable<Token> parameters,
    BlockStatement body)
    : StatementNode(token)
{
    #region Properties

    public BlockStatement Body { get; } = body;

    public IReadOnlyList<Token> Parameters { get; } = [.. parameters];

    #endregion
}