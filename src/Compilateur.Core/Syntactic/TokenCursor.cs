using System.Text;
using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public class TokenCursor : ICursor<Token>
{
    #region Fields

    private static readonly TokenType[] NoSpaceTokens =
        [TokenType.Dot, TokenType.OpenParenthesis, TokenType.Identifier];

    private int _currentIndex;
    private readonly IEnumerable<Token> _tokens;

    #endregion

    #region Constructors

    public TokenCursor(IEnumerable<Token> tokens) => _tokens = tokens;

    #endregion

    #region Properties

    public bool IsAtEnd => Peek().Type == TokenType.Eof;

    #endregion

    #region Methods

    public Token Consume()
    {
        if (!TryConsume(out var token))
        {
            throw new InvalidOperationException(
                $"Unexpected end of token stream at position {_currentIndex}.");
        }

        return token!;
    }

    public bool IsPeekOfType(TokenType tokenType) => Peek().Type == tokenType;
    public bool IsPeekOneOfType(params TokenType[] types) => types.Contains(Peek().Type);

    public Token Peek() => _tokens.ElementAt(_currentIndex);

    public Token? PeekNext() =>
        _currentIndex + 1 >= _tokens.Count()
            ? null
            : _tokens.ElementAt(_currentIndex + 1);

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var token in _tokens)
        {
            builder.Append(token.Lexeme);

            if (!NoSpaceTokens.Contains(token.Type))
            {
                builder.Append(' ');
            }
        }

        builder.AppendLine();

        return builder.ToString();
    }

    public bool TryConsume(out Token? token)
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

    #endregion
}