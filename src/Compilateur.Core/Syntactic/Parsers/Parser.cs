using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic.Parsers;

public abstract class Parser<T> : IParser<T>
    where T : SyntaxNode
{
    #region Methods

    protected abstract T? ParseCore(ParsingContext context);
    public abstract bool Matches(ParsingContext context);

    public T? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            throw new InvalidOperationException(
                $"Parser {GetType().Name} failed to parse lexeme '{context.Cursor.Peek().Lexeme}'."
            );
        }

        return ParseCore(context);
    }

    #endregion
}