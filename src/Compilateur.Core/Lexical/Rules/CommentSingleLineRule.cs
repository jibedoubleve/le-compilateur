using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public record CommentSingleLineRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    private readonly IEnumerable<char> _deadChars = ['\r', '\n'];

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    private bool IsEndOfLine(CodeCursor codeCursor)
    {
        var current = codeCursor.Peek().Char;
        return current.HasValue
               && _deadChars.Contains(current.Value);
    }

    public bool Matches(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return false;
        }

        var current = $"{codeCursor.Peek()}{codeCursor.PeekNext()}";
        return current == "//";
    }

    /// <summary>
    /// Drops the comments, they are not saved
    /// </summary>
    /// <param name="cursor">The token cursor</param>
    /// <param name="errors">Any lexical error is added into this error container</param>
    /// <returns><c>null</c> i any case, the return type is not meant to be used.</returns>
    public Token? Scan(CodeCursor cursor, ErrorCollection? errors = null)
    {
        if (cursor.IsAtEnd)
        {
            return null;
        }

        cursor.Consume(); // Consume '/'
        cursor.Consume(); // Consume second '/'

        for (var i = 0; i < MaxSize; i++)
        {
            if (cursor.IsAtEnd || IsEndOfLine(cursor)) { return null; }

            cursor.Consume();
        }

        return null;
    }

    #endregion
}