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

    public static IEnumerable<object[]> BuildInvalidAst()
    {
        yield return
        [
            """
            {
                var a = 1;
                var a = 2;
            }
            """
        ];
    }

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
            "print b;"
        ];
        yield return
        [
            """
            for (var i = 0; i < 2; i = i + 1)
              print i;
            print i;
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
        yield return
        [
            """
            if (true) print "then";
             else print "else";
            """,
            """
            then

            """
        ];
        yield return
        [
            """
            var i = 0;
            while (i < 3) 
            {
              print i; i = i + 1; 
            }
            """,
            """
            0
            1
            2

            """
        ];
        yield return
        [
            """
            for (var i = 0; i < 2; i = i + 1) 
              print i;
            """,
            """
            0
            1

            """
        ];
        yield return
        [
            """
            fun f(x) 
            {
              print x; 
            }
            f(5);
            """,
            """
            5

            """
        ];
        yield return
        [
            """
            fun f(a)
            {
                print a; 
            }
            {
                var x = 1;
                f(x); 
            }
            """,
            """
            1

            """
        ];
        yield return
        [
            """
            fun f() {}
            {
                var y = 1;
                f();
                print y; 
            }
            """,
            """
            1

            """
        ];
        yield return
        [
            """
            var x = 15;
            fun f()
            {
                print x; 
            }
            {
                var x = 999;
                f(); 
            }
            """,
            """
            15
            
            """
        ];
        yield return
        [
            """
            fun f() {} 
            {
                f(); 
            }
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
            fun f() { return 7; }
            print f();
            """,
            """
            7
            
            """
        ];
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
    [MemberData(nameof(BuildInvalidAst))]
    public void When_Invalid_Ast_Then_Argument_Exception_Is_Thrown(string code)
    {
        // arrange 
        var compiler = new Compiler();

        // act & assert
        var analysis = compiler.Analyse(code);
        Assert.Throws<ArgumentException>(() => compiler.Run(analysis));

        _output.WriteErrors(analysis.Errors);
    }

    [Theory]
    [MemberData(nameof(BuildInvalidCode))]
    public void When_Invalid_Code_Then_Exception_Is_Thrown(string code)
    {
        // arrange 
        var compiler = new Compiler();

        // act & assert
        var analysis = compiler.Analyse(code);
        var ex = Assert.Throws<RuntimeException>(() => compiler.Run(analysis));

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
            var analysis = compiler.Analyse(code);
            compiler.Run(analysis);

            _output.WriteErrors(analysis.Errors);

            // assert
            Assert.Multiple(
                () => analysis.Errors.ShouldBeEmpty(),
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
        var analysis = compiler.Analyse(code);
        compiler.Run(analysis);

        _output.WriteErrors(analysis.Errors);

        // assert
        Assert.Multiple(
            () => analysis.Errors.ShouldBeEmpty(),
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
        var analysis = compiler.Analyse(code);
        compiler.Run(analysis);

        _output.WriteInterpreterContext(code, output, analysis.Errors);

        // assert
        Assert.Multiple(
            () => analysis.Errors.ShouldBeEmpty(),
            () => textWriter.ToString().ShouldBe(output)
        );
    }

    #endregion
}