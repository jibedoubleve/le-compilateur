namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal abstract class PrecedenceParser<TChildParser> : IParser
    where TChildParser : IParser, new()
{
    #region Properties

    protected TChildParser InnerExpression { get; } = new();

    #endregion

    #region Methods

    protected abstract bool MatchesCurrent(ParsingContext context);

    public bool Matches(ParsingContext context) => MatchesCurrent(context) || InnerExpression.Matches(context);

    public abstract SyntaxNode? Parse(ParsingContext context);

    #endregion
}