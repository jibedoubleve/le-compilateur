using Compilateur.Core.Errors;

namespace Compilateur.Core.Syntactic;

public class ParsingContext
{
    #region Constructors

    public ParsingContext(TokenCursor cursor, CompilationErrorCollection? errors = null)
    {
        Cursor = cursor;
        Errors = errors ?? [];
    }

    #endregion

    #region Properties

    public TokenCursor Cursor { get; }

    public CompilationErrorCollection Errors { get; }

    #endregion

    #region Methods

    public void AddError(string message)
        => Errors.Add(
            Cursor.IsEmpty || Cursor.IsAtEnd
                ? new CompilationError(message)
                : new CompilationError(Cursor.Peek(), message)
        );

    #endregion
}