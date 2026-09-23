using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("function")]
public record FunctionDeclarationStatement : StatementNode
{
    #region Constructors

    public FunctionDeclarationStatement(Token token, IEnumerable<ParameterNode> parameters, BlockStatement body) : base(token)
    {
        Parameters = [.. parameters];
        Body = body;
    }

    #endregion

    #region Properties

    public BlockStatement Body { get; }

    public override IEnumerable<SyntaxNode> Children => [Body, .. Parameters];
    public IEnumerable<ParameterNode> Parameters { get; }

    #endregion
}