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

    public Token? Scan(CodeCursor cursor, ErrorCollection? errors = null)
    {
        var first = cursor.Consume();
        var strBuilder = new StringBuilder();

        for (var i = 0; i < MaxSize; i++)
        {
            if (cursor.IsAtEnd)
            {
                errors?.Add(first, "Unterminated string.");
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

        var msg = $"Maximum length of {MaxSize} characters reached.";
        errors?.Add(first, msg);
        return null;

        #endregion
    }
}