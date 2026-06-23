using Compilateur.Core.Syntactic.Rules;
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
    public void When_Statement_Without_Semicolon_Then_Error_Raised()
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
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Valid_Return_Statement_Then_Expected_Tree_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}