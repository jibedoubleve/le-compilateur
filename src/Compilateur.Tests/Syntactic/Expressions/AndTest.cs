using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class AndTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public AndTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_And_Chains_Groups_Then_Expected_Tree_Returned()
    {
        // arrange
        // (a and b) and (c and d) and (1 and 2)
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b =>
                          b.Identifier("a")
                           .And()
                           .Identifier("b"))
                      .And()
                      .BetweenParentheses(b => b.Identifier("c")
                                                .And()
                                                .Identifier("d"))
                      .And()
                      .BetweenParentheses(b => b.Number(1)
                                                .And()
                                                .Number(2))
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Or_Followed_By_And_Then_And_Binds_Tighter()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .Or()
                      .Identifier("b")
                      .And()
                      .Identifier("c")
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