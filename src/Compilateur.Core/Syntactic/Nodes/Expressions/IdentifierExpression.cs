using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("identifier")]
public sealed class IdentifierExpression : ExpressionNode
{
    #region Constructors

    public IdentifierExpression(Token token) : base(token) { }

    #endregion
}