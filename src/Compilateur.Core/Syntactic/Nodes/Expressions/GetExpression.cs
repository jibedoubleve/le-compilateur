using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("get")]
public record GetExpression : ExpressionNode
{
    #region Constructors

    public GetExpression(Token token, ExpressionNode @object) : base(token) => Object = @object;

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Object];

    public ExpressionNode Object { get; }

    #endregion
}