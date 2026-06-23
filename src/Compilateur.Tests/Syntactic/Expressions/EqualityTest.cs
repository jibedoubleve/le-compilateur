using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class EqualityTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public EqualityTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
    {
        // arrange
        // (1 == 1) != (2 == 2) == (3 != 3)
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b => b.Number(1)
                                                .Equality()
                                                .Number(1))
                      .Inequality()
                      .BetweenParentheses(b => b.Number(2)
                                                .Equality()
                                                .Number(2))
                      .Equality()
                      .BetweenParentheses(b => b.Number(3)
                                                .Inequality()
                                                .Number(3))
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