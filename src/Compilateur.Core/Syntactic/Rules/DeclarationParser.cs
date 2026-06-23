using Compilateur.Core.Syntactic.Rules.Declarations;

namespace Compilateur.Core.Syntactic.Rules;

public class DeclarationParser : IParser
{
    #region Fields

    private static readonly IEnumerable<IParser> Parsers =
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

    public SyntaxNode? Parse(ParsingContext context)
    {
        var parser = Parsers.SingleOrDefault(p => p.Matches(context));
        if (parser is null)
        {
            context.AddError($"Expected a declaration or a statement but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        return parser.Parse(context);
    }

    #endregion
}