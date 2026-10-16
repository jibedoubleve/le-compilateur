using Compilateur.Core.Errors;
using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical;

public class Scanner
{
    #region Fields

    private readonly IEnumerable<ITokenRule> _rules;

    #endregion

    #region Constructors

    public Scanner() =>
        _rules =
        [
            new CommentMultiLineRule(),
            new CommentSingleLineRule(),
            new GreaterOrEqualRule(),
            new LessThanOrEqualRule(),
            new EqualityRule(),
            new InequalityRule(),
            new IdentifierRule(),
            new NumericRule(),
            new DotRule(),
            new CommaRule(),
            new SemiColonRule(),
            new OpenBracketRule(),
            new ClosingBracketRule(),
            new OpenCurlyBracketRule(),
            new ClosingBracketRule(),
            new ClosingCurlyBracketRule(),
            new BangRule(),
            new GreaterThanRule(),
            new LessThanOrEqualRule(),
            new LessThanRule(),
            new AssignmentRule(),
            new PlusRule(),
            new MinusRule(),
            new MultiplyRule(),
            new DividedRule(),
            new StringRule(),
            new TriviaRule()
        ];

    #endregion

    #region Methods

    private static void AddEmptyRuleError(CodeCursor cursor, ErrorCollection errors)
    {
        var codeChar = cursor.Consume();
        errors.Add(
            codeChar,
            $"Character '{codeChar.Char}' is not supported"
        );
    }

    public TokenizeResult Tokenize(string source)
    {
        var errors = new ErrorCollection();
        var tokens = new List<Token>();
        var stream = new CodeCursor(source);

        do
        {
            var rule = _rules.Where(r => r.Matches(stream))
                             .OrderByDescending(r => r.Weight)
                             .FirstOrDefault();

            if (rule is null) { AddEmptyRuleError(stream, errors); }
            else
            {
                var token = rule.Scan(stream, errors);
                if (token is not null)
                {
                    tokens.Add(token);
                }
            }
        } while (!stream.IsAtEnd);

        tokens.AppendEof();

        return new TokenizeResult
        {
            Tokens = tokens,
            Errors = errors
        };
    }

    #endregion
}