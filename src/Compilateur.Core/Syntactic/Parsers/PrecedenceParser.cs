using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic.Parsers;

internal abstract class PrecedenceParser<TChildParser> : IParser<ExpressionNode>
    where TChildParser : IParser<ExpressionNode>, new()
{
    #region Properties

    protected TChildParser InnerExpression { get; } = new();

    #endregion

    #region Methods

    protected abstract bool MatchesCurrent(ParsingContext context);

    public bool Matches(ParsingContext context) => MatchesCurrent(context) || InnerExpression.Matches(context);
    public abstract ExpressionNode? Parse(ParsingContext context);

    #endregion
}