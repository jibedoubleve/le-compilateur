using Compilateur.Core.Errors;

namespace Compilateur.Core.Syntactic;

public class ParsingContext
{
    #region Constructors

    public ParsingContext(TokenCursor cursor, SyntaxErrorCollection? errors = null)
    {
        Cursor = cursor;
        Errors = errors ?? [];
    }

    #endregion

    #region Properties

    public TokenCursor Cursor { get; }

    public SyntaxErrorCollection Errors { get; }

    #endregion

    #region Methods

    public void AddError(string message) => Errors.Add(new SyntaxError(Cursor.Peek(), message));

    #endregion
}