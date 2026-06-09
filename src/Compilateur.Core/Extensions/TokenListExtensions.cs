using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Extensions;

public static class TokenListExtensions
{
    #region Methods

    public static void AppendEof(this IList<Token> tokens)
    {
        var lineNumber = tokens.Any()
            ? tokens.Last().Line + 1
            : 1;

        tokens.Add(new Token
        {
            Column = 1,
            Line = lineNumber,
            Type = TokenType.Eof,
            Lexeme = "EOF",
            Value = null
        });
    }

    #endregion
}