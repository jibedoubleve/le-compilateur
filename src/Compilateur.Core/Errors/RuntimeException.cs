namespace Compilateur.Core.Errors;

public class RuntimeException : Exception
{
    #region Constructors

    public RuntimeException(ErrorCollection errors) => Errors = errors;

    #endregion

    #region Properties

    public ErrorCollection Errors { get; }

    #endregion
}