using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
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

    public static IEnumerable<object[]> BuildSimpleOperations()
    {
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .DoubleEqual()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenKind.Equality
        ];
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .Inequality()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext(),
            TokenKind.Inequality
        ];
    }

    [Fact]
    public Task When_Chaining_Equality_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .DoubleEqual()
                      .Identifier("b")
                      .DoubleEqual()
                      .Identifier("c")
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
    {
        // arrange
        // (1 == 1) != (2 == 2) == (3 != 3)
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b => b.Number(1)
                                                .DoubleEqual()
                                                .Number(1))
                      .Inequality()
                      .BetweenParentheses(b => b.Number(2)
                                                .DoubleEqual()
                                                .Number(2))
                      .DoubleEqual()
                      .BetweenParentheses(b => b.Number(3)
                                                .Inequality()
                                                .Number(3))
                      .BuildParsingContext();

        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);

        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildSimpleOperations))]
    public void When_Simple_Operation_Then_Valid_Node_Returned(ParsingContext context, TokenKind tokenKind)
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act

        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // Assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node!.Token.Kind.ShouldBe(tokenKind),
            () => node!.Children.Count().ShouldBe(2)
        );
    }

    #endregion
}