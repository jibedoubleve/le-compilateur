using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("var")]
public record VarDeclarationStatement : StatementNode
{
    #region Constructors

    public VarDeclarationStatement(Token token, ExpressionNode? initialiser = null) : base(token)
        => Initialiser = initialiser;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => Initialiser is null ? [] : [Initialiser];

    public ExpressionNode? Initialiser { get; }

    #endregion
}