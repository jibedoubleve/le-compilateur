using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic;

public class ProgramParserTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ProgramParserTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Token_Stream_Has_No_Eof_Then_Error_Raised()
    {
        // arrange
        var context = TokenCollectionBuilder.BuildEmptyParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Only_Eof_Then_Empty_Program_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
            .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => context.Errors.ShouldBeEmpty()
        );
        return Verify(node);
    }

    [Fact]
    public Task When_Declaration_And_Statement_Then_Program_Contains_Both()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("foo").Equal().Number(1).Semicolon()
                      .Print("hello_world").Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Several_Statements_Then_Program_Contains_All()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("hello_world").Semicolon()
                      .Print("hello_world").Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Single_Statement_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("hello_world")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    #endregion
}