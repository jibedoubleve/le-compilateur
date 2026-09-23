using Compilateur.Core.Lexical.Tokens;
using Compilateur.Tests.Helpers;
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
    public Task When_Comma_Between_Numbers_Then_Not_Part_Of_Number()
    {
        // arrange
        const string code = "f(1,2)";

        // act
        var tokenization = Scanner.Tokenize(code);
        _output.WriteLexerContext(code, tokenization);

        // assert
        return Verify(tokenization.Tokens);
    }

    [Fact]
    public Task When_Number_Has_Several_Dots_Then_Numbers_And_Dots_Returned()
    {
        // arrange
        const string code = "1.2.3.4";

        // act
        var tokenization = Scanner.Tokenize(code);
        _output.WriteLexerContext(code, tokenization);

        // assert
        tokenization.Errors.ShouldBeEmpty();

        return Verify(tokenization.Tokens);
    }

    [Fact]
    public Task When_Number_Ends_With_Dot_Then_Dot_Is_Separate_Token()
    {
        // arrange
        var code = "5.";

        // act
        var tokenization = Scanner.Tokenize(code);
        _output.WriteLexerContext(code, tokenization);
        
        // assert
        return Verify(tokenization.Tokens);
    }

    [Theory]
    [InlineData(" .", TokenKind.Dot, ".")]
    [InlineData(" . ", TokenKind.Dot, ".")]
    [InlineData(". ", TokenKind.Dot, ".")]
    public void When_Whitespace_Around_Token_Then_Whitespace_Is_Ignored(string code, TokenKind tokenKind, string expected)
    {
        var tokenisation = Scanner.Tokenize(code);

        _output.WriteLexerContext(code, tokenisation);

        Assert.Multiple(
            () => tokenisation.HasErrors.ShouldBeFalse(),
            () => tokenisation.Tokens.First().Kind.ShouldBe(tokenKind),
            () => tokenisation.Tokens.First().Lexeme.ShouldBe(expected)
        );
    }

    [Theory]
    [InlineData("²")]
    public void When_Unsupported_Character_Then_Single_Error_Raised(string code)
    {
        var tokenization = Scanner.Tokenize(code);
        _output.WriteLexerContext(code, tokenization);

        tokenization.Errors.ShouldHaveSingleItem();
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
    public void When_Single_Lexeme_Then_Expected_Token_Returned(string code, TokenKind tokenKind)
    {
        var tokenization = Scanner.Tokenize(code);

        _output.WriteLexerContext(code, tokenization);

        Assert.Multiple(
            () => tokenization.HasErrors.ShouldBeFalse(),
            () => tokenization.Tokens.First().Kind.ShouldBe(tokenKind),
            () => tokenization.Tokens.First().Lexeme.ShouldBe(code),
            () => tokenization.Tokens.First().Column.ShouldBe(1),
            () => tokenization.Tokens.First().Line.ShouldBe(1)
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
    public void When_Number_Next_To_Semicolon_Then_Both_Are_Tokens(string code, int count)
    {
        var tokenization = Scanner.Tokenize(code);
        _output.WriteLexerContext(code, tokenization);
        Assert.Multiple(
            () => tokenization.Errors.ShouldBeEmpty(),
            () => tokenization.Tokens.Count.ShouldBe(count + 1) // +1 to add EOF
        );
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("123456789", 123456789)]
    [InlineData("9876.5432", 9876.5432)]
    public void When_Number_Then_Token_Value_Is_Double(string code, double value)
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