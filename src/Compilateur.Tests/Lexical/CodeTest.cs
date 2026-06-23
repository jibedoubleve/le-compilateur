using Compilateur.Core.Extensions;
using Compilateur.Core.Errors.Tokens;
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
    [InlineData("android", new[] { TokenType.Identifier, TokenType.Eof }, new[] { "android", "EOF" })]
    [InlineData("andr+oid",
        new[] { TokenType.Identifier, TokenType.Plus, TokenType.Identifier, TokenType.Eof },
        new[] { "andr", "+", "oid", "EOF" })]
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

    #endregion
}