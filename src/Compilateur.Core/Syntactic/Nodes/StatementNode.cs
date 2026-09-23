using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

[Description("statement")]
public abstract class StatementNode : SyntaxNode
{
    #region Constructors

    protected StatementNode(Token token) : base(token) { }

    #endregion
}