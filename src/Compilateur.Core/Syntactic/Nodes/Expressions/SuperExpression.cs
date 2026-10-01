using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("super")]
public sealed partial class SuperExpression(Token token, Token method) : ExpressionNode(token)
{
    #region Properties

    public Token Method { get; } = method;

    #endregion
}