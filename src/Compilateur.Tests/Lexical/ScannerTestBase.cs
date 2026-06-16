using Compilateur.Core.Extensions;
using Compilateur.Core.Errors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    #region Properties

    protected Scanner Scanner
    {
        get
        {
            var sc = new ServiceCollection();
            var sp = sc.AddLogging(b => b.AddXunit(_output, LogLevel.Debug)
                                         .SetMinimumLevel(LogLevel.Debug))
                       .AddLexicalLayer()
                       .BuildServiceProvider();
            return sp.GetRequiredService<Scanner>();
        }
    }

    #endregion
}