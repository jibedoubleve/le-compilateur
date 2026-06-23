using Compilateur.Core.Syntactic.Rules.Expressions;
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
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
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
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        _output.WriteCode(context);

        // act
        var node = parser.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
        return Verify(node);
    }
    
    [Fact]
    public Task When_Complex_Expression_With_Parenthesis_Parsed_Then_Valid_Node_Returned()
    {
        // arrange
        // a and (b and c) and d and (1 and 2)
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .And()
                      .BetweenParentheses(b =>
                          b.Identifier("b")
                           .And()
                           .Identifier("c"))
                      .And()
                      .Identifier("d")
                      .And()
                      .BetweenParentheses(b => b.Number(1)
                                                .And()
                                                .Number(2))
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        _output.WriteCode(context);

        // act
        var node = parser.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
        return Verify(node);
    }

    #endregion
}