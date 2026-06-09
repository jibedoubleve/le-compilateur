using Compilateur.Core.Extensions;
using Compilateur.Core.Lexer.Tokens;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Lexer;

public class CommentsTests : ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public CommentsTests(ITestOutputHelper output) : base(output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    private void When_Multiline_Comments_Then_They_Are_Ignored()
    {
        const string code = """
                            /* These are comments written
                             * on multiple lines and they
                             * should be ignored during 
                             * the scan...
                             */
                            identifier
                            """;
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.Count.ShouldBe(2),
            () => res.Tokens.First().Lexeme.ShouldBe("identifier")
        );
    }

    [Fact]
    private void When_Multiline_Comments_With_Double_Slash_Then_They_Are_Ignored()
    {
        const string code = """
                            /* These are comments written
                             * on multiple lines and they
                             * should be ignored during 
                             * the scan... Furthermore
                             * single line comments //
                             * should be ignored too
                             */
                            identifier
                            """;
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.Count.ShouldBe(2),
            () => res.Tokens.First().Lexeme.ShouldBe("identifier")
        );
    }

    [Fact]
    private void When_Multiple_Comments_Not_Closed_Then_Error_Raised()
    {
        const string code = """
                            /* If I open multiline comments
                             * and I never close them then
                             * an error should be raised.
                            """;
        var res = Scanner.Tokenize(code);

        _output.WriteLine(res.Errors.Format());

        res.Errors.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData("var android = 42; // Comment will be ignored")]
    [InlineData("""
                // Comment will be ignored
                var android = 42;
                """)]
    [InlineData("""
                // Comment will be ignored
                var android = 42;
                /* comments */
                """)]
    [InlineData("""
                // One line comments
                var android = 42;
                var pi = 3.14;
                var name = "hello world";
                var flag = true;
                var nothing = nil;

                /* multiline comment on one line */
                """)]
    private void When_Single_Line_Comments_After_Lexeme_Then_Token_Is_Generated_And_Comment_Ignored(string code)
    {
        var res = Scanner.Tokenize(code);

        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.First().Lexeme.ShouldBe("var"),
            () => res.Tokens.First().Type.ShouldBe(TokenType.Var)
        );
    }

    [Fact]
    public void When_Single_Line_Comments_Then_They_Are_Ignored()
    {
        const string code = """
                            // Hello World
                            identifier
                            """;
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.Count.ShouldBe(2),
            () => res.Tokens.First().Lexeme.ShouldBe("identifier")
        );
    }

    #endregion
}