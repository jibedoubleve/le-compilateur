using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class WhileStatementParser : IParser
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();
    private static readonly ExpressionStatementParser ExpressionStatementParser = new();
    private static readonly StatementParser StatementParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.While);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError($"Expected 'while' but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        var keyword = SyntaxNode.Unspecified(context.Cursor.Consume());
        var expression = ExpressionParser.Parse(context);
        if (expression is null) { return null; }

        if (StatementParser.Matches(context))
        {
            var block = StatementParser.Parse(context);
            if (block is not null)
            {
                return SyntaxNode.Unspecified(
                    keyword.Token,
                    SyntaxNode.Condition(expression),
                    SyntaxNode.Body(block)
                );
            }
        }

        var expressionStatement = ExpressionStatementParser.Parse(context);
        if (expressionStatement is null) { return null; }

        return SyntaxNode.Unspecified(
            keyword.Token,
            SyntaxNode.Condition(expression),
            SyntaxNode.Body(expressionStatement)
        );
    }

    #endregion
}