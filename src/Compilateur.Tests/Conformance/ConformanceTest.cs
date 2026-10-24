using System.Text;
using Compilateur.Core;
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

    private void OutputInformation(string fileName, string code, string expected, string expectedErrors)
    {
        _output.WriteLine($"""
                           ======== Testing file {fileName} ========
                           """
        );
        _output.WriteLine("""
                          CODE
                          ====
                          """);
        _output.WriteLine(code);

        _output.WriteLine("""
                          Expected output:
                          ================
                          """);
        _output.WriteLine(expected);

        _output.WriteLine("""
                          Expected errors:
                          ================
                          """);
        
        _output.WriteLine(expectedErrors);
    }

    public static IEnumerable<object?[]> GetTest()
    {
        const string path = "../../../Conformance/Lox";
        var files = Directory.GetFiles(path, "*.lox", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var c = new TestParser(file).ReadFile();
            yield return
            [
                Path.GetFileName(file),
                c.Code.ToString(),
                new StringWriter(c.Output),
                c.Errors?.ToString()
            ];
        }
    }

    [Theory]
    [MemberData(nameof(GetTest))]
    public void When_Compiling_Code_Then_Expected_Result_Is_Returned(
        string fileName, string code, StringWriter expectedSw, string expectedErrors)
    {
        // arrange
        var expected = expectedSw.ToString();
        expectedErrors = expectedErrors.TrimEnd();
        
        OutputInformation(
            fileName,
            code,
            expected,
            expectedErrors
        );

        var output = new StringWriter();
        var compiler = new Compiler(output);

        // act
        var analysis = compiler.Analyse(code);

        _output.WriteAnalysisErrors(analysis.Errors);

        if (string.IsNullOrEmpty(expectedErrors))
        {
            compiler.Run(analysis);
        }
        else
        {
            var errors = new StringBuilder();
            _ = analysis.Errors
                        .Select(m => errors.Append(m.Message))
                        .ToList();

            errors.ToString().TrimEnd()
                  .ShouldBe(expectedErrors);
        }

        // assert
        output.ToString().ShouldBe(expected);
    }

    #endregion
}