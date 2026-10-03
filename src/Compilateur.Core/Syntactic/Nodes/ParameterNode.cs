using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

[Description("param")]
public sealed partial class ParameterNode : SyntaxNode
{
    #region Constructors

    public ParameterNode(Token token) : base(token) { }

    #endregion
}