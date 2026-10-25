using System.Text.RegularExpressions;

namespace Compilateur.Tests.Conformance.Helpers;

public static partial class LoxRegex
{
    #region Methods

    [GeneratedRegex("// expect: (.*)")]
    public static partial Regex Expect();

    [GeneratedRegex("// Error at '.*?': (.*)")]
    public static partial Regex ErrorAt();
    
    [GeneratedRegex("// expect runtime error: (.*)")]
    public static partial Regex RuntimeErrorAt();

    #endregion
}