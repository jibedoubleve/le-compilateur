using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Lexer.Rules;

public sealed record TriviaRule : ITokenRule
{
    #region Fields

    private readonly IEnumerable<char> _deadChars = [' ', '\r', '\n'];

    #endregion

    #region Properties

    public int Weight => 1;

    #endregion

    #region Methods

    public bool Matches(CodeStream codeStream)
    {
        var current = codeStream.Peek().Char;
        if (!current.HasValue) return true;

        return _deadChars.Contains(
            current.Value
        );
    }

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        codeStream.Consume();
        return null;
    }

    #endregion
}