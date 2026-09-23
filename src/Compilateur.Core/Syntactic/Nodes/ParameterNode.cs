using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

[Description("param")]
public record ParameterNode : SyntaxNode
{
    #region Constructors

    public ParameterNode(Token token) : base(token) { }

    #endregion
}