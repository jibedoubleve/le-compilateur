using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Core.Syntactic.Rules.Statements;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

public class DeclarationParser : IParser
{
    #region Fields

    private readonly IParser _classDeclaration = new ClassDeclarationParser();
    private readonly IParser _funcDeclaration = new FuncDeclarationParser();
    private readonly IParser _stmtParser = new StatementParser();
    private readonly IParser _varDeclaration = new VarDeclarationParser();

    #endregion

    #region Methods

    public bool Matches(ParsingContext cursor) => _classDeclaration.Matches(cursor)
                                               || _funcDeclaration.Matches(cursor)
                                               || _varDeclaration.Matches(cursor);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (_varDeclaration.Matches(context))
        {
            return _varDeclaration.Parse(context);
        }

        if (_funcDeclaration.Matches(context))
        {
            return _funcDeclaration.Parse(context);
        }

        if (_classDeclaration.Matches(context))
        {
            return _classDeclaration.Parse(context);
        }

        if (_stmtParser.Matches(context))
        {
            _stmtParser.Parse(context);
        }

        var current = context.Cursor.Peek();
        context.AddError($"Unexpected token '{current.Lexeme}' while a declaration is expected.");
        return null;
    }

    #endregion
}