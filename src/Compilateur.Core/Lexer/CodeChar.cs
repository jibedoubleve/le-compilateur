namespace Compilateur.Core.Lexer;

public sealed record CodeChar
{
    #region Properties

    public required char? Char { get; init; }
    public required int Column { get; init; }

    public static CodeChar Empty => new()
    {
        Char = null,
        Column = 0,
        Line = 0
    };

    public bool IsEmpty => Char is null;

    public required int Line { get; init; }

    #endregion

    #region Methods

    public static implicit operator char?(CodeChar codeChar) => codeChar.Char;
    public override string ToString() => Char?.ToString() ?? "";

    #endregion
}