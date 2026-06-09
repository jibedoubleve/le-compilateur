using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public interface ITokenRule
{
    #region Properties

    public int Weight { get; }

    #endregion

    #region Methods

    public bool Matches(CodeStream codeStream);

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null);

    #endregion
}