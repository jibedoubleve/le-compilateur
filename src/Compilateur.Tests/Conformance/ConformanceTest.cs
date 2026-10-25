using System.Text;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Compilateur.Tests.Conformance.Helpers;
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
        string fileName, string code, string output, string? staticErrors, string? runtimeError)
    {
        _output.WriteLine($"""
                           ======== Testing file '{fileName}' ========
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

        if (!string.IsNullOrWhiteSpace(staticErrors))
        {
            _output.WriteLine("""
                              Expected errors:
                              ================
                              """);

            _output.WriteLine(staticErrors);
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


    private static void WithStaticError(Analysis analysis, IEnumerable<StaticError> expectedErrors)
    {
        var expected = expectedErrors.ToList();
        var errors = analysis.Errors.ToList();
        
        analysis.Errors.Count().ShouldBe(expected.Count);
        
        for (var i = 0; i < expected.Count; i++)
        {
            errors[i].Message.ShouldBe(expected[i].Message);
            errors[i].Variable.ShouldBe(expected[i].Variable);
        }
    }

    private static void WithRuntimeError(Compiler compiler, string code, IEnumerable<string> staticError)
    {
        // act
        var analysis = compiler.Analyse(code);
        var ex = Assert.Throws<RuntimeException>(() => compiler.Run(analysis));

        // assert
        var err = staticError.LastOrDefault();
        Assert.Multiple(
            () => err.ShouldNotBeNull(),
            () => ex.Message.ShouldBe(err)
        );
    }

    public static IEnumerable<object?[]> RetrieveTests()
    {
        const string path = "../../../Conformance/Lox";
        foreach (var file in Directory.GetFiles(path, "*.lox", SearchOption.AllDirectories))
        {
            yield return [file];
        }
    }

    [Theory]
    [MemberData(nameof(RetrieveTests))]
    public void When_Compiling_Code_Then_Expected_Result_Is_Returned(string path)
    {
        // arrange
        var (fileName, code, expected, runtimeErrors, staticErrors)
            = LoxFileHelper.ReadFile(path);
        
        staticErrors = staticErrors.ToList();
        runtimeErrors = runtimeErrors.ToList();

        OutputInformation(
            fileName,
            code,
            expected,
            string.Join(Environment.NewLine, runtimeErrors),
            staticErrors.LastOrDefault()?.Message
        );

        var output = new StringWriter();
        var compiler = new Compiler(output);

        // act
        var analysis = compiler.Analyse(code);
        _output.WriteAnalysisErrors(analysis.Errors);
        
        if (staticErrors.Any())
        {
            WithStaticError(analysis, staticErrors);
            return;
        }

        if (runtimeErrors.Any())
        {
            WithRuntimeError(compiler, code, runtimeErrors);
            return;
        }

        compiler.Run(analysis);

        // assert
        output.ToString().ShouldBe(expected);
    }

    #endregion
}