using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class LoopStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public LoopStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_For_Clauses_Are_All_Empty_Then_Loop_Has_No_Initialiser_Condition_Or_Increment()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .For()
                      .BetweenParentheses(b =>
                          b.Semicolon()
                           .Semicolon())
                      .Print("foo bar")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_For_Initialiser_Has_No_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .For()
                      .BetweenParentheses(b =>
                          b.Identifier("foo").Equal().Number(0)
                           .Identifier("foo").LessThanOrEqual().Number(10).Semicolon()
                           .Identifier("foo").Plus().Number(1))
                      .BetweenCurlyBracket(b =>
                          b.Identifier("bar")
                           .Equal()
                           .Number(11).Multiply().Number(60)
                           .Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }


    [Fact]
    public Task When_For_Initialiser_Is_Assignment_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .For()
                      .BetweenParentheses(b =>
                          b.Identifier("foo")
                           .Equal()
                           .Number(0)
                           .Semicolon()
                           .Identifier("foo").LessThanOrEqual().Number(10)
                           .Semicolon()
                           .Identifier("foo").Plus().Number(1))
                      .BetweenCurlyBracket(b =>
                          b.Identifier("bar")
                           .Equal()
                           .Number(11).Multiply().Number(60)
                           .Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_For_Initialiser_Is_Var_And_Body_Is_Block_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .For()
                      .BetweenParentheses(b =>
                          b.Var("foo", c => c.Number(0))
                           .Semicolon()
                           .Identifier("foo").LessThanOrEqual().Number(10)
                           .Semicolon()
                           .Identifier("foo").Plus().Number(1))
                      .BetweenCurlyBracket(b =>
                          b.Identifier("bar")
                           .Equal()
                           .Number(11).Multiply().Number(60)
                           .Semicolon())
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_For_Initialiser_Is_Var_And_Body_Is_Statement_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .For()
                      .BetweenParentheses(b =>
                          b.Var("foo", c => c.Number(0))
                           .Semicolon()
                           .Identifier("foo").LessThanOrEqual().Number(10)
                           .Semicolon()
                           .Identifier("foo").Plus().Number(1))
                      .Identifier("bar")
                      .Equal()
                      .Number(11).Multiply().Number(60)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}