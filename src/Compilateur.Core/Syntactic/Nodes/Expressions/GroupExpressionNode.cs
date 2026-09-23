using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("group")]
public sealed class GroupExpressionNode : ExpressionNode
{
    #region Constructors

    public GroupExpressionNode(Token token, ExpressionNode inner) : base(token) => Inner = inner;

    #endregion

    #region Properties

    public ExpressionNode Inner { get; }

    #endregion
}