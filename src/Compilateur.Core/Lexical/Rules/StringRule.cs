using System.Text;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public sealed record StringRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    public bool Matches(CodeStream codeStream) => codeStream.Peek() == '"';

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var first = codeStream.Consume();
        var strBuilder = new StringBuilder();

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeStream.IsEof)
            {
                errors?.Add(new SyntaxError
                {
                    Column = first.Column,
                    Line = first.Line,
                    Message = "Reached end of file before closing quotes (\")"
                });
                return null;
            }

            if (codeStream.Peek() == '"')
            {
                codeStream.Consume();
                var lexeme = strBuilder.ToString();
                return new Token
                {
                    Lexeme = $"\"{lexeme}\"",
                    Type = TokenType.String,
                    Column = first.Column,
                    Line = first.Line,
                    Value = lexeme
                };
            }

            var next = codeStream.Consume();
            strBuilder.Append(next.Char);
        }

        errors?.Add(new SyntaxError
        {
            Column = first.Column,
            Line = first.Line,
            Message =
                $"String starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
                $"maximum length of {MaxSize} characters."
        });
        return null;
    }

    #endregion
}