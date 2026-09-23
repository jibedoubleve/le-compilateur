using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class ReturnStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ReturnStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Return_Without_Value_Then_Statement_Has_No_Expression()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Return()
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Return_Has_No_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Return()
                      .Identifier("foo")
                      .GreaterThan()
                      .Number(4)
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Return_Has_Value_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Return()
                      .Identifier("foo")
                      .GreaterThan()
                      .Number(4)
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