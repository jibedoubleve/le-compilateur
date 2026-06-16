using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Tests.Helpers;

public static class TokenFactory
{
    #region Methods

    public static IReadOnlyCollection<Token> BuildCollection(int length)
    {
        var list = new List<Token>();
        for (var i = 0; i < length; i++)
        {
            list.Add(new Token
            {
                Column = 0,
                Line = 0,
                Type = TokenType.Identifier,
                Value = "",
                Lexeme = ""
            });
        }
        list.Add(new Token
        {
            Type = TokenType.Eof,
            Line = 0,
            Column = 0,
            Lexeme = ""
        });

        return list;
    }

    #endregion
}