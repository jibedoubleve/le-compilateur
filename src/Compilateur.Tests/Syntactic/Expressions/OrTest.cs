using Compilateur.Core.Syntactic.Rules.Expressions;
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
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
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
        // a or (b or c) or d or (1 or 2)
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .Or()
                      .BetweenParentheses(b =>
                          b.Identifier("b")
                           .Or()
                           .Identifier("c"))
                      .Or()
                      .Identifier("d")
                      .Or()
                      .BetweenParentheses(b => b.Number(1)
                                                .Or()
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