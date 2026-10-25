using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public class ParsingContext
{
    #region Constructors

    public ParsingContext(TokenCursor cursor, ErrorCollection? errors = null)
    {
        Cursor = cursor;
        Errors = errors ?? [];
    }

    #endregion

    #region Properties

    public TokenCursor Cursor { get; }

    public ErrorCollection Errors { get; }

    #endregion

    #region Methods

    public void AddError(string message)
        => Errors.Add(
            Cursor.IsEmpty || Cursor.IsAtEnd
                ? new CompilationError(message)
                : new CompilationError(Cursor.Peek(), message)
        );

    public void AddError(Token token, string message)
        => Errors.Add(new CompilationError(token, message));

    #endregion
}