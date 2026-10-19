namespace Compilateur.Core.Errors;

public class RuntimeException(ErrorCollection errors) : Exception(errors.Last().Message)
{
    #region Properties

    public ErrorCollection Errors { get; } = errors;

    #endregion
}