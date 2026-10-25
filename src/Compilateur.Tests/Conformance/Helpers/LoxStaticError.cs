namespace Compilateur.Tests.Conformance.Helpers;

public record StaticError
{
    #region Constructors

    public StaticError(string message, string variable, int? lineNr)
    {
        Message = message;
        Variable = variable;
        LineNr = lineNr;
    }

    #endregion

    #region Properties

    public int? LineNr { get; }
    public string Message { get; }
    public string? Variable { get; }

    #endregion
}