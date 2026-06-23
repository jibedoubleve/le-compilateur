using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class ReturnStatementParser : IParser
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Return);

    public SyntaxNode? Parse(ParsingContext context)
    {
        var keyword = SyntaxNode.Unspecified(context.Cursor.Consume());
        var expression = ExpressionParser.Parse(context);
        if (expression is null)
        {
            context.AddError($"Expected ')' after expression but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        return SyntaxNode.Unspecified(
            keyword.Token,
            expression
        );
    }

    #endregion
}