using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Parsers.Statements;

namespace Compilateur.Core.Syntactic.Parsers;

public class StatementParser : IParser<StatementNode>
{
    #region Fields

    private static readonly IEnumerable<IParser<StatementNode>> Parsers =
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

    public StatementNode? Parse(ParsingContext context)
    {
        var parser = Parsers.SingleOrDefault(p => p.Matches(context));
        if (parser is not null) { return parser.Parse(context); }

        context.AddError($"Expected a statement, found '{context.Cursor.Peek().Lexeme}'.");
        return null;
    }

    #endregion
}