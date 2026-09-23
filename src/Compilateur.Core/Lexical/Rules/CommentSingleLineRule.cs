using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Lexical.Rules;

public record CommentSingleLineRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    private readonly IEnumerable<char> _deadChars = ['\r', '\n'];
    private readonly ILogger<CommentSingleLineRule> _logger;

    #endregion

    #region Constructors

    public CommentSingleLineRule(ILogger<CommentSingleLineRule> logger) => _logger = logger;

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    private bool IsEndOfLine(CodeCursor codeCursor)
    {
        var current = codeCursor.Peek().Char;
        return current.HasValue
               && _deadChars.Contains(current.Value);
    }

    public bool Matches(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return false;
        }

        var current = $"{codeCursor.Peek()}{codeCursor.PeekNext()}";
        return current == "//";
    }

    public Token? Scan(CodeCursor cursor, SyntaxErrorCollection? errors = null)
    {
        if (cursor.IsAtEnd)
        {
            return null;
        }

        var strBuilder = new StringBuilder();
        strBuilder.Append(cursor.Consume()); // Consume '/'
        strBuilder.Append(cursor.Consume()); // Consume second '/'

        for (var i = 0; i < MaxSize; i++)
        {
            if (cursor.IsAtEnd || IsEndOfLine(cursor))
            {
                _logger.LogDebug("Scanned comments:\n{Comments}", strBuilder.ToString());
                return null;
            }

            var current = cursor.Consume();

            _logger.LogTrace("{Current}", current);

            strBuilder.Append(current);
        }

        return null;
    }

    #endregion
}