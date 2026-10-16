using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class PrintStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var print = context.Cursor.Consume();
        var value = ExpressionParser.Parse(context);
        if (value is null) { return null; }

        return context.ValidateFinalSemicolon(
            new PrintStatement(print, value)
        );
    }

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Print);

    #endregion
}