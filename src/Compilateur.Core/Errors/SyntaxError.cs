using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors;

public sealed record SyntaxError(int Line, int Column, string Message)
{
    #region Constructors

    public SyntaxError(string message) : this(0, 0, message) { }
    public SyntaxError(Token token, string message) : this(token.Line, token.Column, message) { }
    public SyntaxError(CodeChar codeChar, string message) : this(codeChar.Line, codeChar.Column, message) { }

    #endregion
}