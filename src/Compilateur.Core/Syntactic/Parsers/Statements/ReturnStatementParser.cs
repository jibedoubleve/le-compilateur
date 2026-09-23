using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class ReturnStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Return);

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var keyword = context.Cursor.Consume();

        if (context.Cursor.TryConsumeIf(TokenKind.Semicolon))
        {
            return new ReturnStatement(keyword);
        }

        var expression = ExpressionParser.Parse(context);
        if (expression is null) { return null; }

        return context.ValidateFinalSemicolon(
            new ReturnStatement(keyword, expression)
        );
    }

    #endregion
}