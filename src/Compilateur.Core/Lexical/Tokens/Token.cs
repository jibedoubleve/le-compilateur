using System.Diagnostics;

namespace Compilateur.Core.Lexical.Tokens;

[DebuggerDisplay("{Lexeme} [{Kind}]")]
public sealed record Token
{
    #region Properties

    public required int Column { get; init; }
    public required TokenKind Kind { get; init; }
    public required string Lexeme { get; init; }
    public required int Line { get; init; }
    public object? Value { get; init; }

    #endregion
}