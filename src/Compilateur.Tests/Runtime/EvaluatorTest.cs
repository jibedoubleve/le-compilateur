using System.Globalization;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Runtime;

public class EvaluatorTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public EvaluatorTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildInvalidCode()
    {
        yield return
        [
            """
            print 1 + "hello world";
            """
        ];
        yield return
        [
            """
            print 1 - "hello world";
            """
        ];
    }

    public static IEnumerable<object[]> BuildPrintCode()
    {
        const int number = 42;
        const string output = "Some text to output";

        yield return
        [
            $"""
             print "{output}";
             """,
            output
        ];
        yield return
        [
            $"print {number};",
            number
        ];
        yield return
        [
            "print  1 + 5;",
            6
        ];
        yield return
        [
            "print  true;",
            "true"
        ];
        yield return
        [
            "print  false;",
            "false"
        ];
        yield return
        [
            "print  nil;",
            "nil"
        ];
    }

    [Theory]
    [MemberData(nameof(BuildInvalidCode))]
    public void When_Invalid_Code_Then_Exception_Is_Thrown(string code)
    {
        // arrange 
        var compiler = new Compiler();

        // act & assert
        var ex = Assert.Throws<RuntimeException>(() => compiler.Compile(code));

        _output.WriteErrors(ex.Errors);
    }

    [Fact]
    public void When_Print_Decimal_Then_Output_Ignores_Host_Culture()
    {
        const string code = "print 1.5;";

        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

            // arrange

            var textWriter = new StringWriter();
            var compiler = new Compiler(textWriter);

            //act
            var errors = compiler.Compile(code);

            _output.WriteErrors(errors);

            // assert
            Assert.Multiple(
                () => errors.ShouldBeEmpty(),
                () => textWriter.ToString().ShouldBe("1.5\r\n")
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Theory]
    [MemberData(nameof(BuildPrintCode))]
    public void When_Print_Text_Then_Expected_Text(string code, object value)
    {
        // arrange

        var textWriter = new StringWriter();
        var compiler = new Compiler(textWriter);

        //act
        var errors = compiler.Compile(code);

        _output.WriteErrors(errors);

        // assert
        Assert.Multiple(
            () => errors.ShouldBeEmpty(),
            () => textWriter.ToString().ShouldBe($"{value}\r\n")
        );
    }

    #endregion
}