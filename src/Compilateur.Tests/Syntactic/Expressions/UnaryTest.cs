using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class UnaryTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public UnaryTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildNestedUnaryOperators()
    {
        yield return // !!foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Bang()
                                        .Identifier("foo")
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // !-foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Minus()
                                        .Identifier("foo")
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // --foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Minus()
                                        .Identifier("foo")
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // -!foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Bang()
                                        .Number(5)
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // !!foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Bang()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // !-foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Minus()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildSingleUnaryOperators()
    {
        yield return // !foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Identifier("foo")
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // -foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Identifier("foo")
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // !foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // !5
        [
            new TokenCollectionBuilder().Bang()
                                        .Number(5)
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
        yield return // -5
        [
            new TokenCollectionBuilder().Minus()
                                        .Number(5)
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildUnaryWithInvalidOperand()
    {
        yield return // !if
        [
            new TokenCollectionBuilder().Bang()
                                        .If()
                                        .BuildParsingContext()
        ];
        yield return // -;
        [
            new TokenCollectionBuilder().Minus()
                                        .Semicolon()
                                        .BuildParsingContext()
        ];
    }

    [Theory]
    [MemberData(nameof(BuildSingleUnaryOperators))]
    public void When_Single_Unary_Operator_Then_Unary_Node_Returned(ParsingContext context)
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act

        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // Assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node.ShouldBeOfType<UnaryExpression>(),
            () => ((UnaryExpression)node!).Operand.ShouldNotBeNull()
        );
    }

    [Fact]
    public Task When_Unary_Applies_To_Group_Then_Group_Node_Is_Kept()
    {
        // arrange
        // !(!(!a)) 
        var context = new TokenCollectionBuilder().Bang()
                                                  .BetweenParentheses(b =>
                                                      b.Bang()
                                                       .BetweenParentheses(c =>
                                                           c.Bang()
                                                            .Identifier("a")
                                                       )
                                                  )
                                                  .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildUnaryWithInvalidOperand))]
    public void When_Unary_Operand_Is_Invalid_Then_Single_Error_Raised(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.Count().ShouldBe(1)
        );
    }

    [Theory]
    [MemberData(nameof(BuildNestedUnaryOperators))]
    public void When_Unary_Operators_Are_Nested_Then_Unary_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var matches = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        matches.ShouldBeTrue(context.FormatErrors());
        node.ShouldNotBeNull(context.FormatErrors());

        Assert.Multiple(
            () => node.ShouldBeOfType<UnaryExpression>(),
            () => ((UnaryExpression)node).Operand.ShouldNotBeNull(),
            () => node.Token.Kind.ShouldBeOneOf(TokenKind.Bang, TokenKind.Minus)
        );
    }

    #endregion
}