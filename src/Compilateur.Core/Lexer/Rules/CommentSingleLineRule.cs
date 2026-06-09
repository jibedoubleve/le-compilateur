using System.Text;
using Compilateur.Core.Lexer.Tokens;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Lexer.Rules;

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

    private bool IsEndOfLine(CodeStream codeStream)
    {
        var current = codeStream.Peek().Char;
        return current.HasValue
               && _deadChars.Contains(current.Value);
    }

    public bool Matches(CodeStream codeStream)
    {
        if (codeStream.IsEof) return false;

        var current = $"{codeStream.Peek()}{codeStream.PeekNext()}";
        return current == "//";
    }

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        if (codeStream.IsEof) return null;

        var strBuilder = new StringBuilder();
        strBuilder.Append(codeStream.Consume()); // Consume '/'
        strBuilder.Append(codeStream.Consume()); // Consume second '/'

        for (var i = 0; i < MaxSize; i++)
        {
            if (codeStream.IsEof || IsEndOfLine(codeStream))
            {
                _logger.LogDebug("Scanned comments:\n{Comments}", strBuilder.ToString());
                return null;
            }

            var current = codeStream.Consume();

            _logger.LogTrace("{Current}", current);

            strBuilder.Append(current);
        }

        return null;
    }

    #endregion
}