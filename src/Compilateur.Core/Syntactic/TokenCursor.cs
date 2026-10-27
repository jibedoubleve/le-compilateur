using System.Text;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public class TokenCursor : ICursor<Token>
{
    #region Fields

    private int _currentIndex;
    private readonly IEnumerable<Token> _tokens;

    #endregion

    #region Constructors

    public TokenCursor(IEnumerable<Token> tokens) => _tokens = tokens;

    #endregion

    #region Properties

    public bool IsAtEnd => Peek().Kind == TokenKind.Eof;

    public bool IsEmpty => !_tokens.Any();

    #endregion

    #region Methods

    private bool TryConsume(out Token? token)
    {
        if (IsAtEnd)
        {
            token = null;
            return false;
        }

        token = Peek();
        _currentIndex++;
        return true;
    }

    public Token Consume()
    {
        if (!TryConsume(out var token))
        {
            throw new InvalidOperationException(
                $"Unexpected end of token stream at position {_currentIndex}.");
        }

        return token!;
    }

    public Token Peek() => _tokens.ElementAt(_currentIndex);

    public Token? PeekNext() =>
        _currentIndex + 1 >= _tokens.Count()
            ? null
            : _tokens.ElementAt(_currentIndex + 1);

    public void Synchronise() => Panic.Sync(this);

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var token in _tokens)
        {
            builder.Append(token.Lexeme);

            builder.Append(' ');
        }

        builder.AppendLine();

        return builder.ToString();
    }

    #endregion

    private static class Panic
    {
        #region Fields

        private static readonly IReadOnlyCollection<string> StatementStarters =
            ["class", "fun", "var", "for", "if", "while", "print", "return"];

        private const string StatementTerminator = ";";

        #endregion

        #region Methods

        private static bool IsTerminator(Token? token) =>
            string.CompareOrdinal(StatementTerminator, token?.Lexeme ?? string.Empty) == 0;

        public static void Sync(TokenCursor cursor)
        {
            // Consume source of the error
            if (cursor.TryConsume(out var c) && IsTerminator(c))
            {
                return;
            }

            while (!cursor.IsAtEnd)
            {
                var token = cursor.Peek();
                if (StatementStarters.Contains(token.Lexeme)) { return; }

                if (IsTerminator(token))
                {
                    cursor.Consume();
                    return;
                }

                cursor.Consume();
            }
        }

        #endregion
    }
}