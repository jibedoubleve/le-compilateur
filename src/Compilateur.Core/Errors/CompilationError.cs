using Compilateur.Core.Lexical;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors;

public sealed record CompilationError(int Line, int Column, string Message)
{
    #region Constructors

    public CompilationError(string message) : this(0, 0, message) { }
    public CompilationError(Token token, string message) : this(token.Line, token.Column, message) { }
    public CompilationError(CodeChar codeChar, string message) : this(codeChar.Line, codeChar.Column, message) { }

    #endregion
}