using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Parsers.Declarations;

namespace Compilateur.Core.Syntactic.Parsers;

public class DeclarationParser : IParser<StatementNode>
{
    #region Fields

    private static readonly IEnumerable<IParser<StatementNode>> Parsers =
    [
        new ClassDeclarationParser(),
        new FuncDeclarationParser(),
        new StatementParser(),
        new VarDeclarationParser()
    ];

    #endregion

    #region Methods

    public bool Matches(ParsingContext context)
        => Parsers.SingleOrDefault(p => p.Matches(context)) is not null;

    public StatementNode? Parse(ParsingContext context)
    {
        var parser = Parsers.SingleOrDefault(p => p.Matches(context));
        if (parser is null)
        {
            context.AddError($"Expected a declaration or a statement, found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        return parser.Parse(context);
    }

    #endregion
}