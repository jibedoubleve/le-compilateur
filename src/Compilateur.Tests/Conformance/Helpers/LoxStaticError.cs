namespace Compilateur.Tests.Conformance.Helpers;

public record StaticError
{
    #region Constructors

    public StaticError(string message, string variable, int? line)
    {
        Message = message;
        Variable = variable;
        Line = line;
    }

    #endregion

    #region Properties

    public int? Line { get; }
    public string Message { get; }
    public string? Variable { get; }

    #endregion
}