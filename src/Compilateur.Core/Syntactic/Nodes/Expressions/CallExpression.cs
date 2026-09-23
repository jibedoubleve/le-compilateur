using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("call")]
public record CallExpression : ExpressionNode
{
    #region Constructors

    public CallExpression(Token token, ExpressionNode callee, ExpressionNode[] arguments) : base(token)
    {
        Callee = callee;
        Arguments = arguments;
    }

    #endregion

    #region Properties

    public IReadOnlyList<ExpressionNode> Arguments { get; }
    public ExpressionNode Callee { get; }

    public override IEnumerable<SyntaxNode> Children => [Callee, .. Arguments];

    #endregion
}