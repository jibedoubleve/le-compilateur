using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class IfStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    private static readonly StatementParser StatementParser = new();

    #endregion

    #region Methods

    private static bool HasCloseParenthesis(ParsingContext context)
        => context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis);

    private static bool HasOpenParenthesis(ParsingContext context)
        => context.Cursor.IsPeekOfKind(TokenKind.OpenParenthesis);

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.If);

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var keyword = context.Cursor.Consume(); // Consume 'if'
        if (!context.ConsumeOrError(TokenKind.OpenParenthesis)) { return null; }

        /* Handle the condition
         */
        var condition = ExpressionParser.Parse(context);
        if (condition is null) { return null; }

        if (!context.ConsumeOrError(TokenKind.CloseParenthesis)) { return null; }

        /* Handle the THEN branch
         */
        var thenBranch = StatementParser.Parse(context);
        if (thenBranch is null) { return null; }

        /* Handle the ELSE branch
         */
        if (context.Cursor.IsPeekOfKind(TokenKind.Else))
        {
            context.Cursor.Consume(); // consume the 'else'...
            var elseBranch = StatementParser.Parse(context);
            if (elseBranch is not null)
            {
                return new IfStatement(keyword,
                    condition,
                    thenBranch,
                    elseBranch);
            }

            return null;
        }

        return new IfStatement(keyword,
            condition,
            thenBranch);
    }

    #endregion
}