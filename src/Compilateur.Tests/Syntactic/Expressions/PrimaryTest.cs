using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes.Expressions;
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

    public static IEnumerable<object[]> BuildUnclosedGroups()
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

    public static IEnumerable<object[]> BuildNestedGroups()
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
    [MemberData(nameof(BuildUnclosedGroups))]
    public void When_Group_Not_Closed_Then_Error_Raised(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

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
    public void When_Literal_Then_Node_Holds_Its_Value(TokenKind kind, object? value)
    {
        // arrange
        var context = new TokenCollectionBuilder().Value(kind, value)
                                                  .Semicolon()
                                                  .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            ()=> node.ShouldBeOfType<LiteralExpression>(),
            () => ((LiteralExpression)node).Token.Kind.ShouldBe(kind),
            () => ((LiteralExpression)node).Token.Value.ShouldBe(value)
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
    public void When_Token_Kind_Checked_Then_Primary_Parser_Matches_Only_Primary_Tokens(TokenKind kind, bool expected)
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
    [MemberData(nameof(BuildNestedGroups))]
    public void When_Value_Nested_In_Groups_Then_Group_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        _output.WriteSyntaxContext(context, node);
        matched.ShouldBeTrue(context.FormatErrors()); // Expression should be primary expression

        /* No matter how deep is the value nested in parentheses,
         * the value should be returned.
         */
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => node.Token.Kind.ShouldBe(TokenKind.OpenParenthesis),
            () => node.Token.Lexeme.ShouldBe("(")
        );

        context.Cursor.IsAtEnd.ShouldBeTrue(); // All the token should be read
    }

    [Fact]
    public Task When_Super_Member_Access_Then_Expected_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Super()
                      .Dot()
                      .Identifier("foo")
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Super_Without_Dot_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Super()
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.Count().ShouldBe(1)
        );
    }

    [Fact]
    public Task When_This_Member_Access_Then_Expected_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .This()
                      .Dot()
                      .Identifier("foo")
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Super_Dot_Without_Method_Name_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Super()
                      .Dot()
                      .Semicolon()
                      .BuildParsingContext();
        
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.Count().ShouldBe(1)
        );
    }

    [Fact]
    public Task When_This_Without_Member_Then_Expected_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .This()
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}