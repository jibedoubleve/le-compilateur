using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Lexical;

public class CodeTest : ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public CodeTest(ITestOutputHelper output) : base(output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Empty_Code_Then_Eof_Is_Appended()
    {
        // arrange
        const string code = "";

        // act
        var res = Scanner.Tokenize(code);

        _output.WriteLine(res.Errors.Format());

        // assert
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.Last().Type.ShouldBe(TokenType.Eof)
        );
    }

    [Theory]
    [InlineData(-1, "-1")]
    [InlineData(-1.5, "-1,5")]
    public void When_Negative_Number_Then_No_Error_Is_Returned(float number, string expected)
    {
        _output.WriteLine($"Tokenize '{number}'");
        var res = Scanner.Tokenize($"{number}");
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.First().Type.ShouldBe(TokenType.Minus),
            () => res.Tokens.ElementAt(1).Type.ShouldBe(TokenType.Numeric)
        );
    }

    [Theory]
    [InlineData("école")]
    [InlineData("noël")]
    [InlineData("hôtel")]
    public void When_Non_Ascii_In_Identifier_Then_Error_Is_Returned(string code)
    {
        var res = Scanner.Tokenize(code);
        res.Errors.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(1.5, "1,5")]
    [InlineData(0, "0")]
    public void When_Number_Then_No_Error_Is_Returned(float number, string expected)
    {
        _output.WriteLine($"Tokenize '{number}'");
        var res = Scanner.Tokenize($"{number}");
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.First().Type.ShouldBe(TokenType.Numeric),
            () => res.Tokens.First().Lexeme.ShouldBe(expected)
        );
    }

    [Theory]
    [InlineData("android", new[] { TokenType.Identifier, TokenType.Eof }, new[] { "android", "$" })]
    [InlineData("andr+oid",
        new[] { TokenType.Identifier, TokenType.Plus, TokenType.Identifier, TokenType.Eof },
        new[] { "andr", "+", "oid", "$" })]
    public void When_Scan_Identifier_With_Keywords_Then_Identifier_Token_Returned(
        string code, TokenType[] tokenTypes, string[] lexemes)
    {
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Tokens.Count.ShouldBeGreaterThan(0),
            () => res.Tokens.Select(x => x.Type).ShouldBe(tokenTypes),
            () => res.Tokens.Select(x => x.Lexeme).ShouldBe(lexemes)
        );
    }

    [Fact]
    public Task When_Scanning_Code_Then_No_Error_Is_Returned()
    {
        const string code = """
                            // One line comments
                            var android = 42;

                            var pi = 3.14;
                            var name = "hello world";
                            var flag = true;
                            var nothing = nil;

                            /* multiline comment on one line */
                            if (android >= 10 and flag) {
                                print android + pi;
                            } else {
                                print name;
                            }
                            /* Multiline comments
                             * on multiple lines
                             */
                            fun greet(who) {
                                return "hi " + who;
                            }

                            var result = greet(name);
                            print result != "bye";
                            """;

        var res = Scanner.Tokenize(code);

        _output.WriteLine(res.Errors.Format());

        res.Errors.ShouldBeEmpty();
        return Verify(res.Tokens);
    }

    [Theory]
    [InlineData("\"hello")]
    [InlineData("hello\"")]
    public void When_String_Not_Ended_Then_Error_Is_Returned(string code)
    {
        // act
        var res = Scanner.Tokenize(code);
        _output.WriteLine(res.Errors.Format());

        // assert
        res.Errors.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(-0)]
    [InlineData(0)]
    public void When_Zero_Then_No_Error_Is_Returned(float number)
    {
        _output.WriteLine($"Tokenize '{number}'");
        var res = Scanner.Tokenize($"{number}");
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.First().Type.ShouldBe(TokenType.Numeric),
            () => res.Tokens.First().Lexeme.ShouldBe("0")
        );
    }

    #endregion
}