using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
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
    [InlineData(TokenType.Numeric, 1)]
    [InlineData(TokenType.True, true)]
    [InlineData(TokenType.False, false)]
    [InlineData(TokenType.Nil, null)]
    [InlineData(TokenType.String, "Hello world")]
    public void When_Parsing_PrimaryExpression_Has_Value_Then_Node_Returned(TokenType type, object? value)
    {
        // arrange
        var context = new TokenCollectionBuilder().Value(type, value)
                                                  .Semicolon()
                                                  .BuildParsingContext();
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Children.Count().ShouldBe(0),
            () => node.Token.Type.ShouldBe(type),
            () => node.Token.Value.ShouldBe(value)
        );
    }

    [Theory]
    [InlineData(TokenType.Dot, false)]
    [InlineData(TokenType.Comma, false)]
    [InlineData(TokenType.Semicolon, false)]
    [InlineData(TokenType.CloseParenthesis, false)]
    [InlineData(TokenType.OpenCurlyBracket, false)]
    [InlineData(TokenType.CloseCurlyBracket, false)]
    [InlineData(TokenType.Bang, false)]
    [InlineData(TokenType.GreaterThan, false)]
    [InlineData(TokenType.LessThan, false)]
    [InlineData(TokenType.Assignment, false)]
    [InlineData(TokenType.Plus, false)]
    [InlineData(TokenType.Minus, false)]
    [InlineData(TokenType.Multiply, false)]
    [InlineData(TokenType.Divided, false)]
    [InlineData(TokenType.And, false)]
    [InlineData(TokenType.Or, false)]
    [InlineData(TokenType.GreaterThanOrEqual, false)]
    [InlineData(TokenType.LessThanOrEqual, false)]
    [InlineData(TokenType.Equality, false)]
    [InlineData(TokenType.Inequality, false)]
    [InlineData(TokenType.If, false)]
    [InlineData(TokenType.Else, false)]
    [InlineData(TokenType.While, false)]
    [InlineData(TokenType.For, false)]
    [InlineData(TokenType.Fun, false)]
    [InlineData(TokenType.Return, false)]
    [InlineData(TokenType.Class, false)]
    [InlineData(TokenType.Var, false)]
    [InlineData(TokenType.Print, false)]
    [InlineData(TokenType.Eof, false)]
    [InlineData(TokenType.This, true)]
    [InlineData(TokenType.Super, true)]
    [InlineData(TokenType.Numeric, true)]
    [InlineData(TokenType.String, true)]
    [InlineData(TokenType.True, true)]
    [InlineData(TokenType.False, true)]
    [InlineData(TokenType.Nil, true)]
    [InlineData(TokenType.Identifier, true)]
    [InlineData(TokenType.OpenParenthesis, true)]
    public void When_Parsing_PrimaryExpression_Symbol_Then_Match_Accordingly(TokenType type, bool expected)
    {
        // arrange
        var context = new TokenCollectionBuilder().Symbol(type).BuildParsingContext();
        var parser = new PrimaryExpressionParser();

        // act
        var matched = parser.Matches(context);

        // assert
        matched.ShouldBe(
            expected,
            $"The type '{type}' should{(expected ? "" : " NOT")} be supported as an expression."
        );
    }

    [Theory]
    [MemberData(nameof(BuildTokensWithParentheses))]
    public void When_Parsing_PrimaryExpression_With_Parentheses_Then_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        _output.WriteSyntaxTree(node);
        matched.ShouldBeTrue(context.FormatErrors()); // Expression should be primary expression

        /* No matter how deep is the value nested in parentheses,
         * the value should be returned.
         */
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => node.Token.Type.ShouldBe(TokenType.Numeric),
            () => node.Token.Lexeme.ShouldBe("1")
        );

        context.Cursor.IsAtEnd.ShouldBeTrue(); // All the token should be read
    }

    #endregion
}