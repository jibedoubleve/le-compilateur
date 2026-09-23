using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class PrimaryTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public PrimaryTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildInvalidPrimaryExpression()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .OpenParenthesis()
                .Number(1)
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .OpenParenthesis()
                .OpenParenthesis()
                .Number(1)
                .CloseParenthesis()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .OpenParenthesis()
                .OpenParenthesis()
                .Number(1)
                .CloseParenthesis()
                .Plus()
                .Number(4)
                .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildTokensWithParentheses()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .BetweenParentheses(p => p.Number(1))
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .BetweenParentheses(p =>
                    p.BetweenParentheses(q => q.Number(1))
                )
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .BetweenParentheses(p =>
                    p.BetweenParentheses(q =>
                        q.BetweenParentheses(r => r.Number(1))
                    ))
                .BuildParsingContext()
        ];
    }

    [Theory]
    [MemberData(nameof(BuildInvalidPrimaryExpression))]
    public void When_Missing_CloseParenthesis_Then_Raise_Error(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Theory]
    [InlineData(TokenKind.Numeric, 1)]
    [InlineData(TokenKind.True, true)]
    [InlineData(TokenKind.False, false)]
    [InlineData(TokenKind.Nil, null)]
    [InlineData(TokenKind.String, "Hello world")]
    public void When_Parsing_PrimaryExpression_Has_Value_Then_Node_Returned(TokenKind kind, object? value)
    {
        // arrange
        var context = new TokenCollectionBuilder().Value(kind, value)
                                                  .Semicolon()
                                                  .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Children.Count().ShouldBe(0),
            () => node.Token.Kind.ShouldBe(kind),
            () => node.Token.Value.ShouldBe(value)
        );
    }

    [Theory]
    [InlineData(TokenKind.Dot, false)]
    [InlineData(TokenKind.Comma, false)]
    [InlineData(TokenKind.Semicolon, false)]
    [InlineData(TokenKind.CloseParenthesis, false)]
    [InlineData(TokenKind.OpenCurlyBracket, false)]
    [InlineData(TokenKind.CloseCurlyBracket, false)]
    [InlineData(TokenKind.Bang, false)]
    [InlineData(TokenKind.GreaterThan, false)]
    [InlineData(TokenKind.LessThan, false)]
    [InlineData(TokenKind.Assignment, false)]
    [InlineData(TokenKind.Plus, false)]
    [InlineData(TokenKind.Minus, false)]
    [InlineData(TokenKind.Multiply, false)]
    [InlineData(TokenKind.Divided, false)]
    [InlineData(TokenKind.And, false)]
    [InlineData(TokenKind.Or, false)]
    [InlineData(TokenKind.GreaterThanOrEqual, false)]
    [InlineData(TokenKind.LessThanOrEqual, false)]
    [InlineData(TokenKind.Equality, false)]
    [InlineData(TokenKind.Inequality, false)]
    [InlineData(TokenKind.If, false)]
    [InlineData(TokenKind.Else, false)]
    [InlineData(TokenKind.While, false)]
    [InlineData(TokenKind.For, false)]
    [InlineData(TokenKind.Fun, false)]
    [InlineData(TokenKind.Return, false)]
    [InlineData(TokenKind.Class, false)]
    [InlineData(TokenKind.Var, false)]
    [InlineData(TokenKind.Print, false)]
    [InlineData(TokenKind.Eof, false)]
    [InlineData(TokenKind.This, true)]
    [InlineData(TokenKind.Super, true)]
    [InlineData(TokenKind.Numeric, true)]
    [InlineData(TokenKind.String, true)]
    [InlineData(TokenKind.True, true)]
    [InlineData(TokenKind.False, true)]
    [InlineData(TokenKind.Nil, true)]
    [InlineData(TokenKind.Identifier, true)]
    [InlineData(TokenKind.OpenParenthesis, true)]
    public void When_Parsing_PrimaryExpression_Symbol_Then_Match_Accordingly(TokenKind kind, bool expected)
    {
        // arrange
        var context = new TokenCollectionBuilder().Symbol(kind).BuildParsingContext();
        var parser = new PrimaryExpressionParser();

        // act
        var matched = parser.Matches(context);

        // assert
        matched.ShouldBe(
            expected,
            $"The type '{kind}' should{(expected ? "" : " NOT")} be supported as an expression."
        );
    }

    [Theory]
    [MemberData(nameof(BuildTokensWithParentheses))]
    public void When_Parsing_PrimaryExpression_With_Parentheses_Then_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        _output.WriteFullContext(context, node);
        matched.ShouldBeTrue(context.FormatErrors()); // Expression should be primary expression

        /* No matter how deep is the value nested in parentheses,
         * the value should be returned.
         */
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => node.Token.Kind.ShouldBe(TokenKind.Numeric),
            () => node.Token.Lexeme.ShouldBe("1")
        );

        context.Cursor.IsAtEnd.ShouldBeTrue(); // All the token should be read
    }

    #endregion
}