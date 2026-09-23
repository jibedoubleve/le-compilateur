using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Tests.Helpers;

public static class SyntaxNodeFactory
{
    #region Fields

    public const string Lexeme = "Some_Lexeme";

    #endregion

    #region Methods

    private static Token CreateToken(TokenKind? type = null) =>
        new()
        {
            Column = 0,
            Lexeme = Lexeme,
            Line = 0,
            Kind = type ?? TokenKind.Class,
            Value = ""
        };

    #endregion
}