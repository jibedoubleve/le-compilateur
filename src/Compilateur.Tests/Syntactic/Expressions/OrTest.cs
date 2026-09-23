using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Expressions;
using Compilateur.Tests.Helpers;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class OrTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public OrTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Or_Chains_Groups_Then_Expected_Tree_Returned()
    {
        // arrange
        // (a or b) or (c or d) or (1 or 2)
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b =>
                          b.Identifier("a")
                           .Or()
                           .Identifier("b"))
                      .Or()
                      .BetweenParentheses(b => b.Identifier("c")
                                                .Or()
                                                .Identifier("d"))
                      .Or()
                      .BetweenParentheses(b => b.Number(1)
                                                .Or()
                                                .Number(2))
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    #endregion
}