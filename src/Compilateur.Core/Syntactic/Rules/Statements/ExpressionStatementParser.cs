using Compilateur.Core.Syntactic.Helpers;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class ExpressionStatementParser : IParser
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => ExpressionParser.Matches(context);

    public SyntaxNode? Parse(ParsingContext context)
    {
        var expression = ExpressionParser.Parse(context);
        return expression is not null
            ? context.ValidateFinalSemicolon(expression)
            : null;
    }

    #endregion
}