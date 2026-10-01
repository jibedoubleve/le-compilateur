using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical;
using Compilateur.Core.Semantic;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Semantic;

public class SemanticTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public SemanticTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    private Scanner CreateScanner()
    {
        var sc = new ServiceCollection();
        var sp = sc.AddLogging(b => b.AddXunit(_output, LogLevel.Debug)
                                     .SetMinimumLevel(LogLevel.Debug))
                   .AddLexicalLayer()
                   .BuildServiceProvider();
        return sp.GetRequiredService<Scanner>();
    }

    public static IEnumerable<object[]> BuildInvalidCode()
    {
        yield return
        [
            """
            {
                var a = a;
            }
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
        yield return ["return;"];
        yield return ["this;"];
        yield return ["super.A;"];
        yield return ["class A { init() { return 1; } }"];
        yield return ["class A < A { }"];
        yield return ["class A { Foo() { super.B;}  }"];
        yield return
        [
            """
            class A 
            { 
                init() { return 1;}  
            }
            """
        ];
        yield return
        [
            """
            class A {}
            print this;
            """
        ];
        yield return
        [
            """
            fun f() {}
            return;
            """
        ];
        yield return ["var a = this;"];
    }

    public static IEnumerable<object[]> BuildValidCode()
    {
        yield return ["var a = a;", 0];
        yield return
        [
            """
            {
              var x = 1;
              class A {
                m() { print x; }
              }
            }
            """,
            1
        ];
        yield return
        [
            """
            class A < B 
            {
              m() {}
            }
            """,
            0
        ];
        yield return
        [
            """
            class A 
            {
                m() {}
            }
            """,
            0
        ];
        yield return
        [
            "class A { }",
            0
        ];
        yield return
        [
            """
            {
              var a = 1;
              a = 12;
            }
            """,
            1
        ];
        yield return
        [
            """
            fun fib(n) {
              if (n <= 1) return n;
              return fib(n - 2) + fib(n - 1);
            }
            """,
            4
        ];
        yield return
        [
            """
            {
              var i = 0;
              while (i < 10 and !(i == 5)) {
                i = i + 1;
              }
            }
            """,
            4
        ];
        yield return
        [
            """
            {
              class P {}
              var p = P();
              p.x = 1;
              print p.x;
            }
            """,
            3
        ];
        yield return
        [
            """
            class A {
              m() { print this; }
            }
            """,
            1
        ];
        yield return
        [
            """
            class A { m() {} }
            class B < A {
              m() { super.m(); }
            }
            """,
            1
        ];
        yield return
        [
            """
            {
                fun f() {}
                f(); 
            }
            """,
            1
        ];
        yield return // var 'a' is global, this error is caught at runtime
        [
            """
            class A {}
            var a = a;
            """,
            0
        ];
        yield return // var 'a' is global, this error is caught at runtime
        [
            """
            class A {}
            class B < A {}
            var a = a;
            """,
            0
        ];
        yield return
        [
            """
            class A 
            {
                init() { return; } 
            }
            """,
            0
        ];
        yield return
        [
            """
            class B < A 
            {
                m() 
                {
                    super.m(); 
                }
            }
            """,
            1
        ];
    }

    [Theory]
    [MemberData(nameof(BuildInvalidCode))]
    public void When_Binding_Invalid_Code_Then_Expected_Error_Raised(string code)
    {
        // arrange

        // act
        // -- lexer
        var nodes = CreateScanner().Tokenize(code);

        // -- parser
        var cursor = new TokenCursor(nodes.Tokens);
        var context = new ParsingContext(cursor, nodes.Errors);
        var node = ProgramParser.Parse(context);

        _output.WriteSyntaxContext(context, node);

        // -- binder
        var binder = new Binder();
        binder.Visit(node!);

        _output.WriteSemanticContext(binder);

        // assert
        binder.Errors.ShouldNotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(BuildValidCode))]
    public void When_Binding_Valid_Code_Then_Expected_Number_Of_References_Are_Resolved(string code, int count)
    {
        // arrange

        // act
        // -- lexer
        var nodes = CreateScanner().Tokenize(code);

        // -- parser
        var cursor = new TokenCursor(nodes.Tokens);
        var context = new ParsingContext(cursor, nodes.Errors);
        var node = ProgramParser.Parse(context);

        _output.WriteSyntaxContext(context, node);

        // -- binder
        var binder = new Binder();
        binder.Visit(node!);

        _output.WriteSemanticContext(binder);

        // assert
        Assert.Multiple(
            () => binder.DistanceMap.Count.ShouldBe(count),
            () => binder.Errors.ShouldBeEmpty()
        );
    }

    #endregion
}