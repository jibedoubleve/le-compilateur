using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules;

public class ExpressionParser : IParser
{
    #region Fields

    private readonly IParser _innerExpressionParser = new AssignmentExpressionParser();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => _innerExpressionParser.Matches(context);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (Matches(context))
        {
            return _innerExpressionParser.Parse(context);
        }

        context.AddError($"Expected expression, found '{context.Cursor.Peek().Lexeme}'");
        return null;
    }

    #endregion
}