using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Lexical;

public class Scanner
{
    #region Fields

    private readonly ILogger<Scanner> _logger;

    private readonly IEnumerable<ITokenRule> _rules;

    #endregion

    #region Constructors

    public Scanner(IEnumerable<ITokenRule> rules, ILogger<Scanner> logger)
    {
        _rules = rules;
        _logger = logger;
    }

    #endregion

    #region Methods

    private void AddEmptyRuleError(CodeCursor cursor, SyntaxErrorCollection errors)
    {
        var lexeme = cursor.Consume();
        var errorMsg = $"Character '{lexeme.Char}' is not supported";
        errors.Add(new SyntaxError
        {
            Message = errorMsg,
            Column = lexeme.Column,
            Line = lexeme.Line
        });
        _logger.LogWarning(errorMsg);
    }

    public TokenizeResult Tokenize(string source)
    {
        var errors = new SyntaxErrorCollection();
        var tokens = new List<Token>();
        var stream = new CodeCursor(source);

        while (!stream.IsAtEnd)
        {
            var rule = _rules.Where(r => r.Matches(stream))
                             .OrderByDescending(r => r.Weight)
                             .FirstOrDefault();

            if (rule is null) { AddEmptyRuleError(stream, errors); }
            else
            {
                var token = rule.Scan(stream, errors);
                if (token is null)
                {
                    _logger.LogTrace(
                        "[{RuleName}]: {{EMPTY}}",
                        rule.GetType().Name
                    );
                }
                else
                {
                    tokens.Add(token);

                    _logger.LogTrace(
                        "[{RuleName}]: {TokenName}",
                        rule.GetType().Name,
                        token.Lexeme
                    );
                }
            }
        }

        tokens.AppendEof();
        
        return new TokenizeResult
        {
            Tokens = tokens,
            Errors = errors
        };
    }

    #endregion
}