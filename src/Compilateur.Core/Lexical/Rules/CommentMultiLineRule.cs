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
        if (codeCursor.IsAtEnd) return false;

        return $"{codeCursor.Peek()}{codeCursor.PeekNext()}" == "/*";
    }

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var first = codeCursor.Consume(); // Consume '/'
        codeCursor.Consume(); // Consume '*'

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeCursor.IsAtEnd)
            {
                errors?.Add(new SyntaxError
                {
                    Column = first.Column,
                    Line = first.Line,
                    Message = "Unterminated block comment: missing '*/'"
                });
                return null;
            }

            if (EndOfComments(codeCursor))
            {
                codeCursor.Consume(); // Consume '*'
                codeCursor.Consume(); // Consume '/'
                return null;
            }

            codeCursor.Consume();
        }

        errors?.Add(new SyntaxError
        {
            Column = first.Column,
            Line = first.Line,
            Message =
                $"Comments starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
                $"maximum length of {MaxSize} characters."
        });
        return null;
    }

    #endregion
}