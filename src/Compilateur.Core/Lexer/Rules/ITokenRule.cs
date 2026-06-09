using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Lexer.Rules;

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