using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("super")]

public record SuperExpression : ExpressionNode
{
    #region Constructors

    public SuperExpression(Token token, Token method) : base(token) => Method = method;

    #endregion

    #region Properties

    public Token Method { get; }

    #endregion
}