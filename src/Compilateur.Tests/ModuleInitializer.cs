using System.Runtime.CompilerServices;

namespace Compilateur.Tests;

public static class ModuleInitializer
{
    #region Methods

    [ModuleInitializer]
    public static void Initialize() => VerifierSettings.IgnoreMember("Children");

    #endregion
}