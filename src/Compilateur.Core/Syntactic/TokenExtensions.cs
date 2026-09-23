using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic;

public static class TokenExtensions
{
    #region Methods

    public static bool IsOfKind(this SyntaxNode? node, TokenKind tokenKind) =>
        node is not null && node.Token.IsOfKind(tokenKind);

    public static bool IsOfKind(this Token? token, TokenKind tokenKind) => token is not null && token.Kind == tokenKind;

    #endregion
}