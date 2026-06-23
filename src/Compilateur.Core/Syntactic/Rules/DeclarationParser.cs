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
        => Parsers.SingleOrDefault(p => p.Matches(context))
                  ?.Parse(context);

    #endregion
}