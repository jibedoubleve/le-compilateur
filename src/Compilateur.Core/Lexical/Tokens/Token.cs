using System.Diagnostics;
using Compilateur.Core.Errors.Tokens;

namespace Compilateur.Core.Lexical.Tokens;

[DebuggerDisplay("{Lexeme} [{Type}]")]
public sealed record Token
{
    #region Properties

    public required int Column { get; init; }
    public required string Lexeme { get; init; }
    public required int Line { get; init; }
    public required TokenType Type { get; init; }
    public object? Value { get; init; }

    #endregion
}