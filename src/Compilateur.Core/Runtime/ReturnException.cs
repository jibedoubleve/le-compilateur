namespace Compilateur.Core.Runtime;

/// <summary>
///     Exception used pragmatically to break the execution flow when finding a return
/// </summary>
/// <param name="returnValue">The value returned; <c>null</c> if returns nothing</param>
public class ReturnException(object? returnValue) : Exception
{
    #region Properties

    public object? ReturnValue { get; } = returnValue;

    #endregion
}