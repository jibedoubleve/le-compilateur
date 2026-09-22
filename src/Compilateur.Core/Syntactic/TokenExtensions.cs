using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public static class TokenExtensions
{
    #region Methods

    public static bool IsOfType(this SyntaxNode? node, TokenType tokenType) =>
        node is not null && node.Token.IsOfType(tokenType);

    public static bool IsOfType(this Token? token, TokenType tokenType) => token is not null && token.Type == tokenType;

    public static bool IsOneOfRole(this SyntaxNode? node, params SyntaxNodeRole[] types)
        => node is not null && types.Contains(node.Role);

    #endregion
}