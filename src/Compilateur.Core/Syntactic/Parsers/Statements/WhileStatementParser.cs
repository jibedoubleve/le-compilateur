using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class WhileStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();
    private static readonly StatementParser StatementParser = new();

    #endregion

    #region Methods

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.While);

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var keyword = context.Cursor.Consume();
        if (!context.ConsumeOrError(TokenKind.OpenParenthesis)) { return null; }

        var condition = ExpressionParser.Parse(context);
        if (condition is null) { return null; }

        if (!context.ConsumeOrError(TokenKind.CloseParenthesis)) { return null; }

        var statement = StatementParser.Parse(context);
        if (statement is null) { return null; }

        return LoopStatement.While(keyword, condition, statement);
    }

    #endregion
}