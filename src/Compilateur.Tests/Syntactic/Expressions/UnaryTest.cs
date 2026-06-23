using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
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

    public static IEnumerable<object[]> BuildCascadingUnaryOperator()
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

    public static IEnumerable<object[]> BuildInvalidUnaryOperator()
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

    public static IEnumerable<object[]> BuildSimpleUnaryOperator()
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

    [Fact]
    public void When_Cascading_Unary_Parsed_Then_Valid_Node_Returned()
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
        _output.WriteCode(context);

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node!.Token.Type.ShouldBe(TokenType.Bang),
            () => node!.Child(0).Token.Type.ShouldBe(TokenType.Bang),
            () => node!.Child(0).Child(0).Token.Type.ShouldBe(TokenType.Bang)
        );
    }

    [Theory]
    [MemberData(nameof(BuildCascadingUnaryOperator))]
    public void When_Cascading_Unary_Operator_Parsed_Then_Valid_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var matches = parser.Matches(context);
        var node = parser.Parse(context);

        // assert

        _output.WriteSyntaxTree(node);
        matches.ShouldBeTrue(context.FormatErrors());
        node.ShouldNotBeNull(context.FormatErrors());

        Assert.Multiple(
            () => node.Children.Count().ShouldBe(1),
            () => node.Token.Type.ShouldBeOneOf(TokenType.Bang, TokenType.Minus)
        );
    }

    [Theory]
    [MemberData(nameof(BuildInvalidUnaryOperator))]
    public void When_Invalid_Unary_Parsed_Then_Errors_Added(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var match = parser.Matches(context);
        var node = parser.Parse(context);

        _output.WriteLine(context.FormatErrors());
        _output.WriteSyntaxTree(node);

        // assert
        match.ShouldBeTrue();
        context.Errors.Count().ShouldBe(1);
    }

    [Theory]
    [MemberData(nameof(BuildSimpleUnaryOperator))]
    public void When_Simple_Operation_Then_Valid_Node_Returned(ParsingContext context)
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
            () => node!.Children.Count().ShouldBe(1)
        );
    }

    #endregion
}