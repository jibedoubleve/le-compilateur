using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Expressions;
using Compilateur.Tests.Helpers;
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
        yield return
        [
            new TokenCollectionBuilder().Number(1)
                                        .Plus()
                                        .Number(2)
                                        .Semicolon()
                                        .BuildParsingContext()
        ];

        yield return
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

        yield return
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

        yield return
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Minus()
                .Number(2)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return
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

        yield return
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

        yield return
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

        yield return
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
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

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
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Parsing_Algebric_Equation_Then_Algebric_Logic_Is_Used()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Plus()
                      .Number(9)
                      .Multiply()
                      .Number(2)
                      .Plus()
                      .Number(8)
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Two_Numbers_Follows_Then_Maximal_Munch()
    {
        /* This parser should eat the 1+2 and ignore the rest
         * This is expected as the check of EOF or the ';'
         * is a responsibility for another parser (statement) */

        // arrange
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Plus()
                      .Number(2)
                      // Missing term on purpose
                      .Number(3)
                      .Plus()
                      .Number(4)
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => context.Errors.ShouldBeEmpty(),
            () => context.Cursor.Peek().Lexeme.ShouldBe("3")
        );
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildTermOperations))]
    public void When_Valid_Term_Operation_Then_Valid_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var match = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // arrange
        match.ShouldBeTrue();
    }

    #endregion
}