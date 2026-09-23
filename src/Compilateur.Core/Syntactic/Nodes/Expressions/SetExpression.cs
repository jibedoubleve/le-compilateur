using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("set")]
public record SetExpression : ExpressionNode
{
    #region Constructors

    public SetExpression(Token token, ExpressionNode @object, ExpressionNode value) : base(token)
    {
        Object = @object;
        Value = value;
    }

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children => [Object, Value];

    public ExpressionNode Object { get; }
    public ExpressionNode Value { get; }

    #endregion
}