using Compilateur.Core.Syntactic.Rules;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class BlockStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public BlockStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Block_Contains_If_And_Missing_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.If().BetweenParentheses(c => c.True())
                           .BetweenCurlyBracket(d =>
                               d.Print("foo_far")))
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
    public Task When_Block_With_Multiple_Statements_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.Identifier("foo").Plus().Number(5).Semicolon()
                           .Print("Hello_World").Semicolon()
                           .Var("bar").Equal().Number(12).Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Block_With_Simple_Print_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.Print("Hello_World").Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Declaring_Fun_Then_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.Fun("foo")
                           .BetweenCurlyBracket(d =>
                               d.Print("foo_far").Semicolon()))
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Empty_Block_Then_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .OpenCurlyBracket()
                      .CloseCurlyBracket()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Missing_Closing_Bracket_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .OpenCurlyBracket()
                      .Identifier("foo").Plus().Number(5)
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
    public Task When_Recursive_Block_Then_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .OpenCurlyBracket().OpenCurlyBracket()
                      .Identifier("foo").Plus().Number(5).Semicolon()
                      .CloseCurlyBracket().CloseCurlyBracket()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}