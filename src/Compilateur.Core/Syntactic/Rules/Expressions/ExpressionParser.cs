namespace Compilateur.Core.Syntactic.Rules.Expressions;

public class ExpressionParser : IParser
{
    #region Fields

    private readonly IParser _innerExpression = new UnaryExpressionParser();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => _innerExpression.Matches(context);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (Matches(context))
        {
            return _innerExpression.Parse(context);
        }

        context.AddError($"Expected expression, found '{context.Cursor.Peek().Lexeme}'");
        return null;
    }

    #endregion
}