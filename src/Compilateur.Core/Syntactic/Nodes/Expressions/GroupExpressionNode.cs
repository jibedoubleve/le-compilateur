using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

/// <summary>
///     A group is an expression between '(' and ')' used
///     to override operator precedence.
/// </summary>
[Description("group")]
public sealed partial class GroupExpressionNode(Token token, ExpressionNode inner) : ExpressionNode(token)
{
    #region Properties

    public ExpressionNode Inner { get; } = inner;

    #endregion
}