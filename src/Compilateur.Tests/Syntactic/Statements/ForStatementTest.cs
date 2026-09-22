using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class ForStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ForStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Omitting_Semicolon_Then_Error_Raised()
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
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }



    [Fact]
    public Task When_Valid_For_Statement_With_Assignment_Then_Node_Is_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Valid_For_Statement_With_DeclarationBlock_Then_Node_Is_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Valid_For_Statement_With_DeclarationExpression_Then_Node_Is_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}