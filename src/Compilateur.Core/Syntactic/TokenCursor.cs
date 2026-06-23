using System.Text;
using Compilateur.Core.Errors.Tokens;
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

    public bool IsAtEnd => Peek().Type == TokenType.Eof;

    #endregion

    #region Methods

    public Token Consume()
    {
        if (IsAtEnd)
        {
            throw new InvalidOperationException(
                $"Unexpected end of token stream at position {_currentIndex}.");
        }

        var token = Peek();
        _currentIndex++;
        return token;
    }

    public Token Peek() => _tokens.ElementAt(_currentIndex);

    public Token? PeekNext() => _currentIndex + 1 >= _tokens.Count()
        ? null
        : _tokens.ElementAt(_currentIndex + 1);

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
}