using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class ComparisonTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ComparisonTest(ITestOutputHelper output) => _output = output;

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
            TokenKind.GreaterThan
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .GreaterThanOrEqual()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenKind.GreaterThanOrEqual
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .LessThan()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenKind.LessThan
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .LessThanOrEqual()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenKind.LessThanOrEqual
        ];
    }

    [Fact]
    public Task When_Comparison_Chains_Groups_Then_Expected_Tree_Returned()
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

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildSimpleOperations))]
    public void When_Single_Comparison_Then_Binary_Node_Returned(ParsingContext context, TokenKind tokenKind)
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // Assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node.ShouldBeOfType<BinaryExpression>(),
            () => node!.Token.Kind.ShouldBe(tokenKind),
            () => ((BinaryExpression)node!).Left.ShouldNotBeNull(),
            () => ((BinaryExpression)node!).Right.ShouldNotBeNull()
        );
    }

    #endregion
}