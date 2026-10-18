namespace Compilateur.Core.Runtime.Extensions;

public static class ObjectExtension
{
    #region Methods

    public static bool AsBool(this object? value) =>
        value switch
        {
            null  => false,
            false => false,
            _     => true
        };

    #endregion
}