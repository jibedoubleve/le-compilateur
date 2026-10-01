using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("var")]
public sealed partial class VarDeclarationStatement(Token token, ExpressionNode? initialiser = null)
    : StatementNode(token)
{
    #region Properties

    public ExpressionNode? Initialiser { get; } = initialiser;

    #endregion
}