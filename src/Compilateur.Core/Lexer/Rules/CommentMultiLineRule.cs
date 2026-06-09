using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Lexer.Rules;

public record CommentMultiLineRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    #endregion

    #region Properties

    public int Weight { get; } = 999;

    #endregion

    #region Methods

    private bool EndOfComments(CodeStream codeStream)
    {
        var current = $"{codeStream.Peek()}{codeStream.PeekNext()}";
        return current == "*/";
    }

    public bool Matches(CodeStream codeStream)
    {
        if (codeStream.IsEof) return false;

        return $"{codeStream.Peek()}{codeStream.PeekNext()}" == "/*";
    }

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var first = codeStream.Consume(); // Consume '/'
        codeStream.Consume(); // Consume '*'

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeStream.IsEof)
            {
                errors?.Add(new SyntaxError
                {
                    Column = first.Column,
                    Line = first.Line,
                    Message = "Unterminated block comment: missing '*/'"
                });
                return null;
            }

            if (EndOfComments(codeStream))
            {
                codeStream.Consume(); // Consume '*'
                codeStream.Consume(); // Consume '/'
                return null;
            }

            codeStream.Consume();
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