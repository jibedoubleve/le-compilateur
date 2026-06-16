using Compilateur.Core.Errors;
using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public interface ITokenRule
{
    #region Properties

    int Weight { get; }

    #endregion

    #region Methods

    bool Matches(CodeCursor codeCursor);

    Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null);

    #endregion
}