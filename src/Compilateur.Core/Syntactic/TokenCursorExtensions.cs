using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public static class TokenCursorExtensions
{
    extension(TokenCursor cursor)
    {
        #region Methods

        public bool IsPeekOfKind(params TokenKind[] tokenKind) =>
            tokenKind.Contains(
                cursor.Peek().Kind
            );

        public bool TryConsumeIf(TokenKind kind)
        {
            if (!cursor.IsPeekOfKind(kind)) { return false; }

            cursor.Consume();
            return true;
        }

        #endregion
    }
}