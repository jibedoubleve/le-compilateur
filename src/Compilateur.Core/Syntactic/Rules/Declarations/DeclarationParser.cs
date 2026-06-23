using Compilateur.Core.Errors.Tokens;
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

    private static void CheckSemicolon(ParsingContext context)
    {
        if (!context.Cursor.IsPeekOfType(TokenType.Semicolon))
        {
            context.AddError("Expected ';' after statement.");
            return;
        }

        context.Cursor.Consume(); // Consume the ';'
    }

    public bool Matches(ParsingContext cursor) => _classDeclaration.Matches(cursor)
                                                  || _funcDeclaration.Matches(cursor)
                                                  || _varDeclaration.Matches(cursor);

    public SyntaxNode? Parse(ParsingContext context)
    {
        SyntaxNode? result = null;
        if (_varDeclaration.Matches(context))
        {
            result = _varDeclaration.Parse(context);
            CheckSemicolon(context);
        }
        else if (_funcDeclaration.Matches(context))
        {
            result = _funcDeclaration.Parse(context);
        }
        else if (_classDeclaration.Matches(context))
        {
            result = _classDeclaration.Parse(context);
        }
        else if (_stmtParser.Matches(context))
        {
            result = _stmtParser.Parse(context);
        }

        return result;
    }

    #endregion
}