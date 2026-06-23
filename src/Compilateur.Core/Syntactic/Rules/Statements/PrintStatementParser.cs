using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class PrintStatementParser : IParser
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Print);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError($"Expected print but specified '{context.Cursor.Peek().Type}'.");
            return null;
        }

        var print = SyntaxNode.Unspecified(context.Cursor.Consume());

        var value = ExpressionParser.Parse(context);
        if (value is null)
        {
            context.AddError($"Expected expression after 'print' but found '{context.Cursor.Peek().Lexeme}'");
            return null;
        }

        return context.ValidateFinalSemicolon(
            SyntaxNode.Unspecified(print.Token, value)
        );
    }

    #endregion
}