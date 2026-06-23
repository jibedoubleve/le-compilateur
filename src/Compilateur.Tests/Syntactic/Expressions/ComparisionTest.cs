using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class ComparisionTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ComparisionTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildSimpleOperations()
    {
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .GreaterThan()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenType.GreaterThan
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .GreaterThanOrEqual()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenType.GreaterThanOrEqual
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .LessThan()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenType.LessThan
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .LessThanOrEqual()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenType.LessThanOrEqual
        ];
    }

    [Fact]
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
    {
        // arrange
        // (1<2) > (3>4) >= (5<=6)
        var context = new TokenCollectionBuilder().BetweenParentheses(b =>
                                                      b.Number(1)
                                                       .LessThan()
                                                       .Number(2))
                                                  .GreaterThan()
                                                  .BetweenParentheses(b =>
                                                      b.Number(3)
                                                       .GreaterThan()
                                                       .Number(4))
                                                  .GreaterThanOrEqual()
                                                  .BetweenParentheses(b =>
                                                      b.Number(5)
                                                       .LessThanOrEqual()
                                                       .Number(6))
                                                  .BuildParsingContext();
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildSimpleOperations))]
    public void When_Simple_Operation_Then_Valid_Node_Returned(ParsingContext context, TokenType tokenType)
    {
        // Arrange
        var parser = new ExpressionParser();
        _output.WriteCode(context);
        // Act

        var node = parser.Parse(context);
        _output.WriteSyntaxTree(node);

        // Assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node!.Token.Type.ShouldBe(tokenType),
            () => node!.Children.Count().ShouldBe(2)
        );
    }

    #endregion
}