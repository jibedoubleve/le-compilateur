using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers;

public class ExpressionParser : IParser<ExpressionNode>
{
    #region Fields

    private readonly IParser<ExpressionNode> _innerExpressionParser = new AssignmentExpressionParser();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => _innerExpressionParser.Matches(context);

    public ExpressionNode? Parse(ParsingContext context) => _innerExpressionParser.Parse(context);

    #endregion
}