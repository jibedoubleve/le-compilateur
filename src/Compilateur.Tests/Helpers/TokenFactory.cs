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
                Kind = TokenKind.Identifier,
                Value = "",
                Lexeme = ""
            });
        }

        list.Add(new Token
        {
            Kind = TokenKind.Eof,
            Line = 0,
            Column = 0,
            Lexeme = "$"
        });

        return list;
    }

    #endregion
}