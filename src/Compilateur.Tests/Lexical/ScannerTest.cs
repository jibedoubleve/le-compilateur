using Compilateur.Core.Extensions;
using Compilateur.Core.Errors.Tokens;
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
    [InlineData(" .", TokenType.Dot, ".")]
    [InlineData(" . ", TokenType.Dot, ".")]
    [InlineData(". ", TokenType.Dot, ".")]
    public void When_Scan_Contains_Dead_Char_Then_They_Are_Ignored(string code, TokenType tokenType, string expected)
    {
        var res = Scanner.Tokenize(code);

        _output.WriteLine($"{res.Errors.Format()}");

        Assert.Multiple(
            () => res.HasErrors.ShouldBeFalse(),
            () => res.Tokens.First().Type.ShouldBe(tokenType),
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
    [InlineData(".", TokenType.Dot)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData("(", TokenType.OpenParenthesis)]
    [InlineData(")", TokenType.CloseParenthesis)]
    [InlineData("{", TokenType.OpenCurlyBracket)]
    [InlineData("}", TokenType.CloseCurlyBracket)]
    [InlineData("!", TokenType.Bang)]
    [InlineData(">", TokenType.GreaterThan)]
    [InlineData("<", TokenType.LessThan)]
    [InlineData("=", TokenType.Assignment)]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Multiply)]
    [InlineData("/", TokenType.Divided)]
    // Double chars
    [InlineData("||", TokenType.Or)]
    [InlineData("&&", TokenType.And)]
    [InlineData(">=", TokenType.GreaterOrEqual)]
    [InlineData("<=", TokenType.LessThanOrEqual)]
    [InlineData("==", TokenType.Equality)]
    [InlineData("!=", TokenType.Inequality)]
    // Identifiers
    [InlineData("one_two", TokenType.Identifier)]
    [InlineData("_one_1", TokenType.Identifier)]
    [InlineData("one", TokenType.Identifier)]
    // Keywords
    [InlineData("and", TokenType.And)]
    [InlineData("or", TokenType.Or)]
    [InlineData("nil", TokenType.Nil)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("while", TokenType.While)]
    [InlineData("for", TokenType.For)]
    [InlineData("fun", TokenType.Fun)]
    [InlineData("var", TokenType.Var)]
    [InlineData("class", TokenType.Class)]
    [InlineData("this", TokenType.This)]
    [InlineData("super", TokenType.Super)]
    [InlineData("return", TokenType.Return)]
    [InlineData("true", TokenType.True)]
    [InlineData("false", TokenType.False)]
    [InlineData("print", TokenType.Print)]
    // Numbers
    [InlineData("0", TokenType.Numeric)]
    [InlineData("123456789", TokenType.Numeric)]
    [InlineData("1234.56789", TokenType.Numeric)]
    // Strings
    [InlineData("\"undeux\"", TokenType.String)]
    public void When_Scan_Lexeme_Then_Expected_Token_Returned(string code, TokenType tokenType)
    {
        var res = Scanner.Tokenize(code);

        _output.WriteLine($"{res.Errors.Format()}");

        Assert.Multiple(
            () => res.HasErrors.ShouldBeFalse(),
            () => res.Tokens.First().Type.ShouldBe(tokenType),
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
            () => res.Tokens.First().Type.ShouldBe(TokenType.Numeric)
        );
    }

    #endregion
}