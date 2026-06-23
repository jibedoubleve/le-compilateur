using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Statements;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class PrintStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public PrintStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_No_Print_Then_Parser_Return_No_Node()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .String("print")
                      .String("Hello world")
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new PrintStatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Print_Expression_Then_Node_Is_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Symbol(TokenType.Print)
                      .Number(1).Plus().Number(2)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Theory]
    [InlineData("foobar")]
    [InlineData("hello world")]
    [InlineData("hello\nworld")]
    [InlineData("hello_world")]
    [InlineData("1 + 4")]
    public void When_Print_String_Then_Node_Is_Returned(string output)
    {
        // arrange
        output = $"\"{output}\"";
        var context = new TokenCollectionBuilder()
                      .Print(output)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => context.Errors.ShouldBeEmpty()
        );
    }

    [Fact]
    public void When_Statement_Has_No_Final_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("Hello world")
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public void When_Statement_Start_Without_Expression_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Symbol(TokenType.Print)
                      .Bang()
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    #endregion
}