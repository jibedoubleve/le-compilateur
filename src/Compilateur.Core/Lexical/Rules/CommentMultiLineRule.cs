using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public record CommentMultiLineRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    #endregion

    #region Properties

    public int Weight { get; } = 999;

    #endregion

    #region Methods

    private bool EndOfComments(CodeCursor codeCursor)
    {
        var current = $"{codeCursor.Peek()}{codeCursor.PeekNext()}";
        return current == "*/";
    }

    public bool Matches(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return false;
        }

        return $"{codeCursor.Peek()}{codeCursor.PeekNext()}" == "/*";
    }

    public Token? Scan(CodeCursor cursor, CompilationErrorCollection? errors = null)
    {
        var first = cursor.Consume(); // Consume '/'
        cursor.Consume(); // Consume '*'

        for (var i = 0; i < MaxSize; i++)
        {
            if (cursor.IsAtEnd)
            {
                errors?.Add(first, "Unterminated block comment: missing '*/'");
                return null;
            }

            if (EndOfComments(cursor))
            {
                cursor.Consume(); // Consume '*'
                cursor.Consume(); // Consume '/'
                return null;
            }

            cursor.Consume();
        }

        var msg =
            $"Comments starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
            $"maximum length of {MaxSize} characters.";

        errors?.Add(first, msg);
        return null;
    }

    #endregion
}