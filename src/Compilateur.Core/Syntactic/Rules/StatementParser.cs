using Compilateur.Core.Syntactic.Rules.Statements;

namespace Compilateur.Core.Syntactic.Rules;

public class StatementParser : IParser
{
    #region Fields

    private static readonly IEnumerable<IParser> Parsers =
    [
        new BlockStatementParser(),
        new ExpressionStatementParser(),
        new IfStatementParser(),
        new PrintStatementParser(),
        new WhileStatementParser(),
        new ForStatementParser(),
        new ReturnStatementParser()
    ];

    #endregion

    #region Methods

    public bool Matches(ParsingContext context)
        => Parsers.SingleOrDefault(p => p.Matches(context)) is not null;

    public SyntaxNode? Parse(ParsingContext context)
        => Parsers.SingleOrDefault(p => p.Matches(context))
                  ?.Parse(context);

    #endregion
}