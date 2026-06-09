namespace Compilateur.Core.Lexer;

public sealed record SyntaxError
{
    #region Properties

    public required int Column { get; init; }
    public required int Line { get; init; }
    public required string Message { get; init; }

    #endregion
}