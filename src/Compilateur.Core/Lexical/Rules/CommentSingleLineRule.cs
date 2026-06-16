using System.Text;
using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Errors.Rules;

public record CommentSingleLineRule : ITokenRule
{
    #region Fields

    private readonly IEnumerable<char> _deadChars = ['\r', '\n'];
    private readonly ILogger<CommentSingleLineRule> _logger;

    private const int MaxSize = 10_000;

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
        if (codeCursor.IsAtEnd) return false;

        var current = $"{codeCursor.Peek()}{codeCursor.PeekNext()}";
        return current == "//";
    }

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        if (codeCursor.IsAtEnd) return null;

        var strBuilder = new StringBuilder();
        strBuilder.Append(codeCursor.Consume()); // Consume '/'
        strBuilder.Append(codeCursor.Consume()); // Consume second '/'

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeCursor.IsAtEnd || IsEndOfLine(codeCursor))
            {
                _logger.LogDebug("Scanned comments:\n{Comments}", strBuilder.ToString());
                return null;
            }

            var current = codeCursor.Consume();

            _logger.LogTrace("{Current}", current);

            strBuilder.Append(current);
        }

        return null;
    }

    #endregion
}