using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;

namespace Compilateur.Tests.Syntactic;

public partial class ExpressionTests
{
    #region Methods

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
    [InlineData(TokenType.Dot)]
    [InlineData(TokenType.Comma)]
    [InlineData(TokenType.Semicolon)]
    [InlineData(TokenType.CloseParenthesis)]
    [InlineData(TokenType.OpenCurlyBracket)]
    [InlineData(TokenType.CloseCurlyBracket)]
    [InlineData(TokenType.Bang)]
    [InlineData(TokenType.GreaterThan)]
    [InlineData(TokenType.LessThan)]
    [InlineData(TokenType.Assignment)]
    [InlineData(TokenType.Plus)]
    [InlineData(TokenType.Minus)]
    [InlineData(TokenType.Multiply)]
    [InlineData(TokenType.Divided)]
    [InlineData(TokenType.And)]
    [InlineData(TokenType.Or)]
    [InlineData(TokenType.GreaterOrEqual)]
    [InlineData(TokenType.LessThanOrEqual)]
    [InlineData(TokenType.Equality)]
    [InlineData(TokenType.Inequality)]
    [InlineData(TokenType.If)]
    [InlineData(TokenType.Else)]
    [InlineData(TokenType.While)]
    [InlineData(TokenType.For)]
    [InlineData(TokenType.Fun)]
    [InlineData(TokenType.Return)]
    [InlineData(TokenType.Class)]
    [InlineData(TokenType.This)]
    [InlineData(TokenType.Super)]
    [InlineData(TokenType.Var)]
    [InlineData(TokenType.Print)]
    [InlineData(TokenType.Eof)]
    public void When_Parsing_PrimaryExpression_Without_Supported_Symbol_Then_No_Match(TokenType type)
    {
        // arrange
        var context = Tokens.Symbol(type).BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);

        // assert
        matched.ShouldBeFalse();
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
        matched.ShouldBeTrue(); // Expression should be primary expression

        /* No matter how deep is the value nested in parentheses,
         * the value should be returned.
         */
        node.ShouldNotBeNull();
        Assert.Multiple(
            () => node.Token.Type.ShouldBe(TokenType.Numeric),
            () => node.Token.Lexeme.ShouldBe("1")
        );

        context.Cursor.IsAtEnd.ShouldBeTrue(); // All the token should be read
    }

    [Theory]
    [InlineData(TokenType.Numeric, 1)]
    [InlineData(TokenType.True, true)]
    [InlineData(TokenType.False, false)]
    [InlineData(TokenType.Nil, null)]
    [InlineData(TokenType.String, "Hello world")]
    public void When_Parsing_PrimaryExpression_Has_Value_Then_Node_Returned(TokenType type, object? value)
    {
        // arrange
        var context = Tokens.Value(type, value).BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        node.ShouldNotBeNull();
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Children.Count().ShouldBe(0),
            () => node.Token.Type.ShouldBe(type),
            () => node.Token.Value.ShouldBe(value)
        );
    }

    #endregion
}