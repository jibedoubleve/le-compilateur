using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public static class TokenCursorExtensions
{
    extension(TokenCursor cursor)
    {
        #region Methods

        public bool IsPeekNextOfKind(TokenKind tokenKind) => cursor.PeekNext()?.Kind == tokenKind;

        public bool IsPeekOfKind(TokenKind tokenKind) => cursor.Peek().Kind == tokenKind;

        public bool TryConsumeIf(TokenKind kind)
        {
            if (!cursor.IsPeekOfKind(kind)) { return false; }

            cursor.Consume();
            return true;
        }

        #endregion
    }
}