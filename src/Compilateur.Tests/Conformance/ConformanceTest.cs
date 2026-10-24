using System.Text;
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

    private void OutputInformation(
        string fileName, string code, string output, string errors, string? runtimeError)
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

        if (!string.IsNullOrWhiteSpace(output))
        {
            _output.WriteLine("""
                              Expected output:
                              ================
                              """);
            _output.WriteLine(output);
        }

        if (!string.IsNullOrWhiteSpace(errors))
        {
            _output.WriteLine("""
                              Expected errors:
                              ================
                              """);

            _output.WriteLine(errors);
        }

        if (!string.IsNullOrWhiteSpace(runtimeError))
        {
            _output.WriteLine("""
                              Expected runtime errors:
                              ========================
                              """);

            _output.WriteLine(runtimeError);
        }
    }


    private static void WithCompilationError(Analysis analysis, string expectedErrors)
    {
        var retrievedErrors = new StringBuilder();
        _ = analysis.Errors
                    .Select(m => retrievedErrors.Append(m.Message))
                    .ToList();

        retrievedErrors.ToString().TrimEnd()
                       .ShouldBe(expectedErrors);
    }

    private static void WithRuntimeError(Compiler compiler, string code, string runtimeError)
    {
        // act
        var analysis = compiler.Analyse(code);
        var ex = Assert.Throws<RuntimeException>(() => compiler.Run(analysis));

        // assert
        ex.Message.ShouldBe(runtimeError);
    }

    public static IEnumerable<object?[]> RetrieveTests()
    {
        const string path = "../../../Conformance/Lox";
        foreach (var file in Directory.GetFiles(path, "*.lox", SearchOption.AllDirectories))
        {
            var c = file.ReadFile();
            yield return
            [
                c.FileName,
                c.Code.ToString(),
                c.Output.ToString(),
                c.Errors?.ToString(),
                c.RuntimeError
            ];
        }
    }

    [Theory]
    [MemberData(nameof(RetrieveTests))]
    public void When_Compiling_Code_Then_Expected_Result_Is_Returned(
        string fileName, string code, string expectedOutput, string compilationErrors, string? runtimeError)
    {
        // arrange
        var expected = new StringWriter(new StringBuilder(expectedOutput));
        compilationErrors = compilationErrors.TrimEnd();

        OutputInformation(
            fileName,
            code,
            expected.ToString(),
            compilationErrors,
            runtimeError
        );

        var output = new StringWriter();
        var compiler = new Compiler(output);

        // act
        if (!string.IsNullOrWhiteSpace(runtimeError))
        {
            WithRuntimeError(compiler, code, runtimeError);
            return;
        }

        var analysis = compiler.Analyse(code);

        _output.WriteAnalysisErrors(analysis.Errors);

        if (!string.IsNullOrEmpty(compilationErrors))
        {
            WithCompilationError(analysis, compilationErrors);
            return;
        }

        compiler.Run(analysis);

        // assert
        output.ToString().ShouldBe(expected.ToString());
    }

    #endregion
}