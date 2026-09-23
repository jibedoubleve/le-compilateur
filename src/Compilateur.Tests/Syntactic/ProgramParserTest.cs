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
    public void When_Empty_Code_Then_Error_Raised()
    {
        // arrange
        var context = TokenCollectionBuilder.BuildEmptyParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Empty_Code_With_Eof_Then_Empty_Tree()
    {
        // arrange
        var context = new TokenCollectionBuilder()
            .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => context.Errors.ShouldBeEmpty()
        );
        return Verify(node);
    }

    [Fact]
    public Task When_Mix_Of_Statements_And_Expressions_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("foo").Equal().Number(1).Semicolon()
                      .Print("hello_world").Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Multiple_Statement_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("hello_world").Semicolon()
                      .Print("hello_world").Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Orphan_Token_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .OpenCurlyBracket()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Single_Statement_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("hello_world")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}