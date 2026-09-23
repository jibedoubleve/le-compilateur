using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

public sealed class LiteralExpression : ExpressionNode
{
    #region Constructors

    public LiteralExpression(Token token) : base(token) { }

    #endregion
}