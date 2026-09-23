using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class CallTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public CallTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildDotExpression()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .Identifier("a")
                .Dot().Identifier("b")
                .Dot().Identifier("C")
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Identifier("a")
                .Dot().Identifier("b")
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Identifier("a")
                .Dot().Identifier("b").EmptyCall()
                .Dot().Identifier("c")
                .BuildParsingContext()
        ];
    }

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

    public static IEnumerable<object[]> BuildValidCallExpressionTokenWithTreeInformation()
    {
        const string foo = "foo";
        const string a = "a";
        const string b = "b";

        // foo
        yield return
        [
            new TokenCollectionBuilder().Identifier(foo)
                                        .EmptyCall()
                                        .BuildParsingContext(),
            0
        ];

        // foo()
        yield return
        [
            new TokenCollectionBuilder().Identifier(foo)
                                        .EmptyCall()
                                        .Semicolon()
                                        .BuildParsingContext(),
            0
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
                                        .Semicolon()
                                        .BuildParsingContext(),
            2
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
                                        .Semicolon()
                                        .BuildParsingContext(),
            1
        ];
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
        yield return // foo.bar
        [
            new TokenCollectionBuilder().Identifier("foo").Dot()
                                        .Identifier("bar")
                                        .BuildParsingContext()
        ];

        yield return // foo.bar.baz
        [
            new TokenCollectionBuilder().Identifier("foo").Dot()
                                        .Identifier("bar").Dot()
                                        .Identifier("baz")
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

    [Theory]
    [MemberData(nameof(BuildDotExpression))]
    public void When_Call_Dot_Then_No_Error(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => context.Errors.ShouldBeEmpty(),
            () => node.ShouldNotBeNull(),
            () => node!.Child(0).Children.ShouldNotBeNull()
        );
    }

    [Fact]
    public Task When_Call_Dot_Then_Tree_Is_Correct()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo")
                      .Dot()
                      .Identifier("bar")
                      .BuildParsingContext();
        var parser = new ExpressionParser();
        // act

        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Chained_Call_Then_Tree_Is_Correct()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo")
                      .EmptyCall()
                      .EmptyCall()
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        context.Errors.ShouldBeEmpty();
        return Verify(node);
    }

    [Fact]
    public Task When_Chained_Call_With_Arguments_Then_Tree_Is_Correct()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo")
                      .BetweenParentheses(b => b.Identifier("a").Comma().Identifier("b"))
                      .BetweenParentheses(b => b.Identifier("c").Comma().Identifier("d"))
                      .EmptyCall()
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        context.Errors.ShouldBeEmpty();
        return Verify(node);
    }

    [Fact]
    public Task When_Chained_Dot_Call_Then_Tree_Is_Correct()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo").Dot()
                      .Identifier("bar").Dot()
                      .Identifier("baz")
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Chaining_Call_Then_Expected_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo")
                      .EmptyCall()
                      .EmptyCall()
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Chaining_Call_With_Parameters_Then_Expected_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("foo")
                      .BetweenParentheses(b => b.Number(1))
                      .BetweenParentheses(b => b.Number(2))
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
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
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node.ShouldBeOfType<CallExpression>(),
            () => (node as CallExpression)!.Arguments.Count.ShouldBe(nodeCount)
        );
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
        var node = expression.Parse(context);
        _output.WriteFullContext(context, node);

        // arrange
        match.ShouldBeTrue();
        context.Errors.ShouldBeEmpty();
    }

    #endregion
}