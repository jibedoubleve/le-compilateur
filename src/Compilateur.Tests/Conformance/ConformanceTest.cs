using Compilateur.Core;
using Compilateur.Core.Errors;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Conformance;

public class ConformanceTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ConformanceTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object?[]> GetTest()
    {
        const string path = "../../../Conformance/Lox";
        var files = Directory.GetFiles(path, "*.lox", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var c = new TestParser(file).ReadFile();
            yield return
            [
                c.Code.ToString(),
                new StringWriter(c.Output),
                null
            ];
        }
    }

    [Theory]
    [MemberData(nameof(GetTest))]
    public void When_Compiling_Code_Then_Expected_Result_Is_Returned(
        string code, StringWriter expected, RuntimeException? expectedException)
    {
        // arrange
        _output.WriteLine("""
                          CODE
                          ====
                          """);
        _output.WriteLine(code);

        _output.WriteLine("""
                          Expected output:
                          ================
                          """);
        _output.WriteLine(expected.ToString());

        var output = new StringWriter();
        var compiler = new Compiler(output);
        
        // act
        var analysis = compiler.Analyse(code);
        
        _output.WriteAnalysisErrors(analysis.Errors);
        
        compiler.Run(analysis);

        // assert
        output.ToString().ShouldBe(expected.ToString());

    }

    #endregion
}