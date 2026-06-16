using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;

namespace Compilateur.Tests.Syntactic;

public partial class ExpressionTests
{
    #region Methods

    public static IEnumerable<object[]> BuildCascadingUnaryOperator()
    {
        yield return // !!foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Bang()
                                        .Identifier("foo")
                                        .BuildParsingContext()
        ];
        yield return // !-foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Minus()
                                        .Identifier("foo")
                                        .BuildParsingContext()
        ];
        yield return // --foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Minus()
                                        .Identifier("foo")
                                        .BuildParsingContext()
        ];
        yield return // -!foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Bang()
                                        .Number(5)
                                        .BuildParsingContext()
        ];
        yield return // !!foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Bang()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
        yield return // !-foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Minus()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildSimpleUnaryOperator()
    {
        yield return // !foo
        [
            new TokenCollectionBuilder().Bang()
                                        .Identifier("foo")
                                        .BuildParsingContext()
        ];
        yield return // -foo
        [
            new TokenCollectionBuilder().Minus()
                                        .Identifier("foo")
                                        .BuildParsingContext()
        ];
        yield return // !foo()
        [
            new TokenCollectionBuilder().Bang()
                                        .Identifier("foo")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
        yield return // !5
        [
            new TokenCollectionBuilder().Bang()
                                        .Number(5)
                                        .BuildParsingContext()
        ];
        yield return // -5
        [
            new TokenCollectionBuilder().Minus()
                                        .Number(5)
                                        .BuildParsingContext()
        ];
    }

    [Theory]
    [MemberData(nameof(BuildCascadingUnaryOperator))]
    [MemberData(nameof(BuildSimpleUnaryOperator))]
    public void When_Cascading_Unary_Operator_Parsed_Then_Valid_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var matches = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        
        _output.WriteLine(context.Errors.Format());
        matches.ShouldBeTrue();
        node.ShouldNotBeNull();

        Assert.Multiple(
            () => node.Children.Count().ShouldBe(1),
            () => node.Token.Type.ShouldBeOneOf(TokenType.Bang, TokenType.Minus)
        );
    }

    [Theory]
    [MemberData(nameof(BuildCascadingUnaryOperator))]
    public void When_Cascading_Unary_Operator_Parsed_Then_Child_Is_Unary_Operator(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();
        
        // act
        var node = parser.Parse(context);

        // assert

        _output.WriteLine(context.Errors.Format());
        node.ShouldNotBeNull();

        Assert.Multiple(
            () => node.Children.Count().ShouldBe(1),
            () => node.Children.First().Token.Type.ShouldBeOneOf(TokenType.Bang, TokenType.Minus)
        );
    }

    #endregion
}