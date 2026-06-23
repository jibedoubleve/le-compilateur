using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Compilateur.Tests.Lexical;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class TermTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public TermTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildTermOperations()
    {
        yield return // 1 * 2
        [
            new TokenCollectionBuilder().Number(1)
                                        .Plus()
                                        .Number(2)
                                        .BuildParsingContext()
        ];

        yield return // 1 * (2 * 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Plus()
                .BetweenParentheses(p =>
                    p.Number(2)
                     .Plus()
                     .Number(3)
                )
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 * 2 * 3
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Plus()
                .Number(2)
                .Plus()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / 2
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Minus()
                .Number(2)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / 2 / 3
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Minus()
                .Number(2)
                .Minus()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / (2 / 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Minus()
                .BetweenParentheses(p => p.Number(2)
                                          .Minus()
                                          .Number(3)
                )
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / (2 * 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Minus()
                .Number(2)
                .Plus()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];
        yield return // (1 + 2) + (1 + 2) - (1 - 2) 
        [
            new TokenCollectionBuilder()
                .BetweenParentheses(b =>
                    b.Number(1)
                     .Minus()
                     .Number(2)
                )
                .Plus()
                .BetweenParentheses(b =>
                    b.Number(1)
                     .Plus()
                     .Number(2)
                )
                .Minus()
                .BetweenParentheses(b =>
                    b.Number(1)
                     .Plus()
                     .Number(2)
                )
                .Semicolon()
                .BuildParsingContext()
        ];
    }

    [Fact]
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
    {
        // arrange

        // 10 - 3 - 2
        var context = new TokenCollectionBuilder()
                      .Number(10)
                      .Minus()
                      .Number(3)
                      .Minus()
                      .Number(2)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        _output.WriteCode(context);
        var node = parser.Parse(context);

        _output.WriteSyntaxTree(node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Complex_Term_Operation_Then_No_Compilation_Error()
    {
        // arrange

        // 10 - 3 - 2
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Minus().Number(2)
                      .Minus().Number(3)
                      .Minus().Number(4)
                      .Minus().Number(5)
                      .Minus().Number(6)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        _output.WriteCode(context);
        var node = parser.Parse(context);

        _output.WriteSyntaxTree(node);

        // assert
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildTermOperations))]
    public void When_Valid_Term_Operation_Then_Valid_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        _output.WriteCode(context);
        var match = parser.Matches(context);
        var node = parser.Parse(context);

        _output.WriteSyntaxTree(node);

        // arrange
        match.ShouldBeTrue();
    }

    #endregion
}