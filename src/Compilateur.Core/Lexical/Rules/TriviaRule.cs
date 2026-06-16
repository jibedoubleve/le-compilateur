using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors.Rules;

public sealed record TriviaRule : ITokenRule
{
    #region Fields

    private readonly IEnumerable<char> _deadChars = [' ', '\r', '\n'];

    #endregion

    #region Properties

    public int Weight => 1;

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor)
    {
        var current = codeCursor.Peek().Char;
        if (!current.HasValue) return true;

        return _deadChars.Contains(
            current.Value
        );
    }

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        codeCursor.Consume();
        return null;
    }

    #endregion
}