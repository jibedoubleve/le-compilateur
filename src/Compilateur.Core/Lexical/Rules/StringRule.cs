using System.Text;
using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors.Rules;

public sealed record StringRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor) => codeCursor.Peek() == '"';

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var first = codeCursor.Consume();
        var strBuilder = new StringBuilder();

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeCursor.IsAtEnd)
            {
                errors?.Add(new SyntaxError(first, "Reached end of file before closing quotes (\")"));
                return null;
            }

            if (codeCursor.Peek() == '"')
            {
                codeCursor.Consume();
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

            var next = codeCursor.Consume();
            strBuilder.Append(next.Char);
        }

        var msg =
            $"String starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
            $"maximum length of {MaxSize} characters.";
        errors?.Add(new SyntaxError(first, msg));
        return null;

        #endregion
    }
}