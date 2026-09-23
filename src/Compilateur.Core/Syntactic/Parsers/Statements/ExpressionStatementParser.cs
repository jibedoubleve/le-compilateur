using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Compilateur.Core.Syntactic.Parsers.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class ExpressionStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public override bool Matches(ParsingContext context) => ExpressionParser.Matches(context);

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var expression = ExpressionParser.Parse(context);
        return expression is not null
            ? context.ValidateFinalSemicolon(new ExpressionStatement(expression))
            : null;
    }

    #endregion
}