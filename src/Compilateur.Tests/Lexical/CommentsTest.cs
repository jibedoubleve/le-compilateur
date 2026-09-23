using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Lexical;

public class CommentsTest : ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public CommentsTest(ITestOutputHelper output) : base(output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Block_Comment_Then_It_Is_Ignored()
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
    public void When_Block_Comment_Contains_Double_Slash_Then_It_Is_Ignored()
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
    public void When_Block_Comment_Not_Closed_Then_Error_Raised()
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
    public void When_Comments_Around_Code_Then_Code_Tokens_Are_Kept(string code)
    {
        var res = Scanner.Tokenize(code);

        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.ShouldNotBeEmpty(),
            () => res.Tokens.First().Lexeme.ShouldBe("var"),
            () => res.Tokens.First().Kind.ShouldBe(TokenKind.Var)
        );
    }

    [Fact]
    public void When_Line_Comment_Then_It_Is_Ignored()
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