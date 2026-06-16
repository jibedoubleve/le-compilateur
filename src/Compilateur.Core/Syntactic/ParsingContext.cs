using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

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
    public void AddError(Token token, string message) => Errors.Add(new SyntaxError(token, message));

    #endregion
}