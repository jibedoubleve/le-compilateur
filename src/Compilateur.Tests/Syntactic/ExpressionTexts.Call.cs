using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;

namespace Compilateur.Tests.Syntactic;

public partial class ExpressionTests
{
    #region Methods

    public static IEnumerable<object[]> BuildInvalidCallExpressionTokens()
    {
        yield return [new TokenCollectionBuilder().Semicolon().BuildParsingContext()];
        yield return
        [
            new TokenCollectionBuilder()
                .Semicolon()
                .Semicolon()
                .BuildParsingContext()
        ];
        yield return [new TokenCollectionBuilder().If().BuildParsingContext()];
        yield return [new TokenCollectionBuilder().Else().BuildParsingContext()];
        yield return [new TokenCollectionBuilder().While().BuildParsingContext()];
        yield return [new TokenCollectionBuilder().For().BuildParsingContext()];
        yield return [new TokenCollectionBuilder().Return().BuildParsingContext()];
    }

    public static IEnumerable<object[]> BuildValidCallExpressionTokens()
    {
        yield return // foo()
        [
            new TokenCollectionBuilder().Identifier("foo")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
        yield return // foo
        [
            new TokenCollectionBuilder().Identifier("foo")
                                        .BuildParsingContext()
        ];

        yield return // foo(1 + 2)
        [
            new TokenCollectionBuilder().Identifier("foo")
                                        .BetweenParentheses(p =>
                                            p.Number(1)
                                             .Plus()
                                             .Number(2))
                                        .BuildParsingContext()
        ];

        yield return // foo(bar())
        [
            new TokenCollectionBuilder().Identifier("foo")
                                        .BetweenParentheses(p =>
                                            p.Identifier("bar")
                                             .EmptyCall())
                                        .BuildParsingContext()
        ];

        yield return // foo(bar(), 2)
        [
            new TokenCollectionBuilder().Identifier("foo")
                                        .BetweenParentheses(p =>
                                            p.Identifier("bar")
                                             .EmptyCall()
                                             .Comma()
                                             .Number(2))
                                        .BuildParsingContext()
        ];
        yield return // Semantic is invalid, but syntax is valid
        [
            new TokenCollectionBuilder().Identifier("5")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
        yield return // Semantic is invalid, but syntax is valid
        [
            new TokenCollectionBuilder().Identifier("nil")
                                        .EmptyCall()
                                        .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildValidCallExpressionTokenWithTreeInformation()
    {
        const string foo = "foo";
        const string a = "a";
        const string b = "b";

        // foo
        yield return [new TokenCollectionBuilder().Identifier(foo).BuildParsingContext(), 0];
        // foo()
        yield return
        [
            new TokenCollectionBuilder().Identifier(foo)
                                        .EmptyCall()
                                        .BuildParsingContext(),
            1
        ];
        // foo(a, b)
        yield return
        [
            new TokenCollectionBuilder().Identifier(foo)
                                        .BetweenParentheses(p => {
                                            p.Identifier(a)
                                             .Comma()
                                             .Identifier(b);
                                        })
                                        .BuildParsingContext(),
            3
        ];
        // foo(1 + 2)
        yield return
        [
            new TokenCollectionBuilder().Identifier(foo)
                                        .BetweenParentheses(p => {
                                            p.Number(1)
                                             .Plus()
                                             .Number(2);
                                        })
                                        .BuildParsingContext(),
            2
        ];
    }

    [Theory]
    [MemberData(nameof(BuildValidCallExpressionTokenWithTreeInformation))]
    public void When_Parsing_CallExpression_Then_Expected_Node_Returned(ParsingContext context, int nodeCount)
    {
        // arrange
        _output.WriteLine($"Output tokens: {context.Cursor}");
        var expression = new ExpressionParser();

        // act
        var node = expression.Parse(context);

        _output.WriteLine(context.Errors.Format());
        
        // assert
        node.ShouldNotBeNull();
        node!.Children.Count().ShouldBe(nodeCount);
    }

    [Theory]
    [MemberData(nameof(BuildInvalidCallExpressionTokens))]
    public void When_Parsing_Invalid_CallExpression_Then_No_Match(ParsingContext context)
    {
        // arrange
        var expression = new ExpressionParser();

        // act 
        var match = expression.Matches(context);

        // arrange
        match.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(BuildValidCallExpressionTokens))]
    public void When_Parsing_Valid_CallExpression_Then_Matches(ParsingContext context)
    {
        // arrange
        var expression = new ExpressionParser();

        // act 
        var match = expression.Matches(context);

        // arrange
        match.ShouldBeTrue();
    }

    #endregion
}