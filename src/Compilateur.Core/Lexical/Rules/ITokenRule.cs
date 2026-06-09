using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public interface ITokenRule
{
    #region Properties

    public int Weight { get; }

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor);

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null);

    #endregion
}