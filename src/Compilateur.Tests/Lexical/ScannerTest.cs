using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Lexical;

public class ScannerTest : ScannerTestBase
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ScannerTest(ITestOutputHelper output) : base(output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Multiple_Separator_In_Number_Then_Error_Is_Raised()
    {
        const string code = "1.2.3.4";

        var res = Scanner.Tokenize(code);

        res.Errors.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(" .", TokenKind.Dot, ".")]
    [InlineData(" . ", TokenKind.Dot, ".")]
    [InlineData(". ", TokenKind.Dot, ".")]
    public void When_Scan_Contains_Dead_Char_Then_They_Are_Ignored(string code, TokenKind tokenKind, string expected)
    {
        var res = Scanner.Tokenize(code);

        _output.WriteLine($"{res.Errors.Format()}");

        Assert.Multiple(
            () => res.HasErrors.ShouldBeFalse(),
            () => res.Tokens.First().Kind.ShouldBe(tokenKind),
            () => res.Tokens.First().Lexeme.ShouldBe(expected)
        );
    }

    [Theory]
    [InlineData("²")]
    public void When_Scan_Find_Unsupported_Lexeme_Then_Error_Is_Written(string code)
    {
        var res = Scanner.Tokenize(code);
        foreach (var error in res.Errors)
        {
            _output.WriteLine($"ERROR: {error.Message} (Line {error.Line}, Column {error.Column})");
        }

        res.Errors.ShouldHaveSingleItem();
    }

    [Theory]
    // Single char
    [InlineData(".", TokenKind.Dot)]
    [InlineData(",", TokenKind.Comma)]
    [InlineData(";", TokenKind.Semicolon)]
    [InlineData("(", TokenKind.OpenParenthesis)]
    [InlineData(")", TokenKind.CloseParenthesis)]
    [InlineData("{", TokenKind.OpenCurlyBracket)]
    [InlineData("}", TokenKind.CloseCurlyBracket)]
    [InlineData("!", TokenKind.Bang)]
    [InlineData(">", TokenKind.GreaterThan)]
    [InlineData("<", TokenKind.LessThan)]
    [InlineData("=", TokenKind.Assignment)]
    [InlineData("+", TokenKind.Plus)]
    [InlineData("-", TokenKind.Minus)]
    [InlineData("*", TokenKind.Multiply)]
    [InlineData("/", TokenKind.Divided)]
    // Double chars
    [InlineData(">=", TokenKind.GreaterThanOrEqual)]
    [InlineData("<=", TokenKind.LessThanOrEqual)]
    [InlineData("==", TokenKind.Equality)]
    [InlineData("!=", TokenKind.Inequality)]
    // Identifiers
    [InlineData("one_two", TokenKind.Identifier)]
    [InlineData("_one_1", TokenKind.Identifier)]
    [InlineData("one", TokenKind.Identifier)]
    // Keywords
    [InlineData("and", TokenKind.And)]
    [InlineData("or", TokenKind.Or)]
    [InlineData("nil", TokenKind.Nil)]
    [InlineData("if", TokenKind.If)]
    [InlineData("else", TokenKind.Else)]
    [InlineData("while", TokenKind.While)]
    [InlineData("for", TokenKind.For)]
    [InlineData("fun", TokenKind.Fun)]
    [InlineData("var", TokenKind.Var)]
    [InlineData("class", TokenKind.Class)]
    [InlineData("this", TokenKind.This)]
    [InlineData("super", TokenKind.Super)]
    [InlineData("return", TokenKind.Return)]
    [InlineData("true", TokenKind.True)]
    [InlineData("false", TokenKind.False)]
    [InlineData("print", TokenKind.Print)]
    // Numbers
    [InlineData("0", TokenKind.Numeric)]
    [InlineData("123456789", TokenKind.Numeric)]
    [InlineData("1234.56789", TokenKind.Numeric)]
    // Strings
    [InlineData("\"undeux\"", TokenKind.String)]
    public void When_Scan_Lexeme_Then_Expected_Token_Returned(string code, TokenKind tokenKind)
    {
        var res = Scanner.Tokenize(code);

        _output.WriteLine($"{res.Errors.Format()}");

        Assert.Multiple(
            () => res.HasErrors.ShouldBeFalse(),
            () => res.Tokens.First().Kind.ShouldBe(tokenKind),
            () => res.Tokens.First().Lexeme.ShouldBe(code),
            () => res.Tokens.First().Column.ShouldBe(1),
            () => res.Tokens.First().Line.ShouldBe(1)
        );
    }

    [Theory]
    [InlineData("1;", 2)]
    [InlineData("2.3;", 2)]
    [InlineData("4;5", 3)]
    [InlineData("4.1;5.1", 3)]
    [InlineData(";6", 2)]
    [InlineData(";6.1", 2)]
    [InlineData("8.9", 1)]
    [InlineData("8", 1)]
    public void When_Scan_Number_With_Semicolon_Then_SemiColum_Is_Ignored(string code, int count)
    {
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.Count.ShouldBe(count + 1) // +1 to add EOF
        );
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("123456789", 123456789)]
    [InlineData("9876.5432", 9876.5432)]
    public void When_Scan_Numeric_Then_Value_Is_Numeric(string code, double value)
    {
        var res = Scanner.Tokenize(code);
        Assert.Multiple(
            () => res.Errors.ShouldBeEmpty(),
            () => res.Tokens.Count.ShouldBe(2), // The numeric value and the EOF
            () => ((double)res.Tokens.First().Value!).ShouldBe(value, 1e-9),
            () => res.Tokens.First().Kind.ShouldBe(TokenKind.Numeric)
        );
    }

    #endregion
}