using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

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

    public bool Matches(CodeCursor codeCursor) => codeCursor.Peek() == '"';

    public Token? Scan(CodeCursor cursor, SyntaxErrorCollection? errors = null)
    {
        var first = cursor.Consume();
        var strBuilder = new StringBuilder();

        for (var i = 0; i < MaxSize; i++)
        {
            if (cursor.IsAtEnd)
            {
                errors?.Add(first, "Reached end of file before closing quotes (\")");
                return null;
            }

            if (cursor.Peek() == '"')
            {
                cursor.Consume();
                var lexeme = strBuilder.ToString();
                return new Token
                {
                    Lexeme = $"\"{lexeme}\"",
                    Kind = TokenKind.String,
                    Column = first.Column,
                    Line = first.Line,
                    Value = lexeme
                };
            }

            var next = cursor.Consume();
            strBuilder.Append(next.Char);
        }

        var msg =
            $"String starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
            $"maximum length of {MaxSize} characters.";
        errors?.Add(first, msg);
        return null;

        #endregion
    }
}