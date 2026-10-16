using Xunit.Abstractions;

namespace Compilateur.Tests.Lexical;

public class ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    protected ScannerTestBase(ITestOutputHelper output) => _output = output;

    #endregion
}