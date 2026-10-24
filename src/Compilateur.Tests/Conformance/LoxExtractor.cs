using System.Text.RegularExpressions;

namespace Compilateur.Tests.Conformance;

public static partial class LoxExtractor
{
    #region Methods

    [GeneratedRegex("// expect: (.*)")]
    public static partial Regex Expect();

    #endregion
}