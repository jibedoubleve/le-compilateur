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
        yield return
        [
            """
            {
                var a = 1;
                var a = 2;
            }
            """
        ];

        yield return
        [
            """
            print b;
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

    public static IEnumerable<object[]> BuildValidCode()
    {
        yield return
        [
            """
            print -3;
            print !true;
            """,
            """
            -3
            false

            """
        ];
        yield return
        [
            "print (1 + 2) * 3;",
            """
            9

            """
        ];
        yield return
        [
            """
            print nil or "ok";
            print false and 1;
            """,
            """
            ok
            false

            """
        ];
        yield return
        [
            """
            var a = 1;
            print a;
            """,
            """
            1

            """
        ];
        yield return
        [
            """
            var a = 1;
            var a = 2;
            print a;
            """,
            """
            2

            """
        ];

        yield return
        [
            """
            var a;
            print a;
            """,
            """
            nil

            """
        ];
        yield return
        [
            """
            var a = 1;
            a = 2; 
            print a;
            """,
            """
            2

            """
        ];
        yield return
        [
            """
            var a = 1;
            a = a + 1;
            print a;
            """,
            """
            2

            """
        ];
        yield return
        [
            """
            var a = 1;
            {
                var a = 2;
                print a;
            }
            print a;
            """,
            """
            2
            1

            """
        ];
        yield return
        [
            """
            var a = 1;
            {
              a = 2;
            }
            print a;
            """,
            """
            2

            """
        ];
        yield return
        [
            """
            var a = 1; 
            print a = 2;
            """,
            """
            2
            
            """
        ];
//         yield return
//         [
//             """
//             if (true) print "then";
//              else print "else";
//             """,
//             "then"
//         ];
//         yield return
//         [
//             """
//             var i = 0;
//             while (i < 3) 
//             {
//               print i; i = i + 1; 
//             }
//             """,
//             "0 1 2"
//         ];
//         yield return
//         [
//             """
//             for (var i = 0; i < 2; i = i + 1) 
//               print i;
//             """,
//             "0 1"
//         ];
//         yield return
//         [
//             """
//             fun f(x) 
//             {
//               print x; 
//             }
//             f(5);
//             """,
//             "5"
//         ];
//         yield return
//         [
//             """
//             fun f() { return 7; }
//             print f();
//             """,
//             "7"
//         ];
//         yield return
//         [
//             """
//             class A {} 
//             print A;
//             """,
//             "A"
//         ];
//         yield return
//         [
//             """
//             class A {}
//             print A();
//             """,
//             "A instance"
//         ];
//         yield return
//         [
//             """
//             class A {}
//             var a = A();
//             a.x = 3;
//             print a.x;
//             """,
//             "3"
//         ];
//         yield return
//         [
//             """
//             class A
//             { 
//               m()
//               {
//                   return this.v;
//               }
//             }
//             var a = A();
//             a.v = 4;
//             print a.m();
//             """,
//             "4"
//         ];
//         yield return
//         [
//             """
//             class A
//             {
//                 m() 
//                 {
//                    return "A"; 
//                 } 
//             } 
//             class B < A 
//             { 
//                 m() 
//                 {
//                     return super.m();
//                 }
//             } 
//
//             print B().m();
//             """,
//             "A"
//         ];
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

    [Theory]
    [MemberData(nameof(BuildValidCode))]
    public void When_Valid_Code_Then_No_Error(string code, string output)
    {
        // arrange 
        output = output.ReplaceLineEndings();
        var textWriter = new StringWriter();
        var compiler = new Compiler(textWriter);

        // act & assert
        var compilation = compiler.Compile(code);

        _output.WriteInterpreterContext(code, output, compilation);

        // assert
        Assert.Multiple(
            () => compilation.ShouldBeEmpty(),
            () => textWriter.ToString().ShouldBe(output)
        );
    }

    #endregion
}