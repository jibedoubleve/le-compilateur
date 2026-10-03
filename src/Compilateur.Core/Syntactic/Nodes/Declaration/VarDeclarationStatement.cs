using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("var")]
public sealed partial class VarDeclarationStatement : StatementNode
{
    #region Constructors

    public VarDeclarationStatement(Token token, ExpressionNode? initialiser = null) : base(token)
        => Initialiser = initialiser;

    #endregion

    #region Properties

    public ExpressionNode? Initialiser { get; }

    #endregion
}