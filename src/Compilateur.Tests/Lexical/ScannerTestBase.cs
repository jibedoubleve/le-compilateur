
namespace Compilateur.Tests.Lexical;

[Trait("Lexical", "Lexical")]
public class ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    protected ScannerTestBase(ITestOutputHelper output) => _output = output;

    #endregion
}