using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class FactorTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public FactorTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildSimpleFactorOperator()
    {
        yield return // 1 * 2
        [
            new TokenCollectionBuilder().Number(1)
                                        .Multiply()
                                        .Number(2)
                                        .BuildParsingContext()
        ];

        yield return // 1 * (2 * 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Multiply()
                .BetweenParentheses(p =>
                    p.Number(2)
                     .Multiply()
                     .Number(3)
                )
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 * 2 * 3
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Multiply()
                .Number(2)
                .Multiply()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / 2
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Divided()
                .Number(2)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / 2 / 3
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Divided()
                .Number(2)
                .Divided()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / (2 / 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Divided()
                .BetweenParentheses(p => p.Number(2)
                                          .Divided()
                                          .Number(3)
                )
                .Semicolon()
                .BuildParsingContext()
        ];

        yield return // 1 / (2 * 3)
        [
            new TokenCollectionBuilder()
                .Number(1)
                .Divided()
                .Number(2)
                .Multiply()
                .Number(3)
                .Semicolon()
                .BuildParsingContext()
        ];
    }

    [Fact]
    public Task When_Complex_Factor_Operation_Then_All_Nodes_Are_Processed()
    {
        // arrange

        // 10 / 3 / 2
        var context = new TokenCollectionBuilder()
                      .Number(10)
                      .Divided()
                      .Number(3)
                      .Divided()
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

    [Theory]
    [MemberData(nameof(BuildSimpleFactorOperator))]
    public void When_Valid_Factor_Operation_Then_Valid_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();

        // act
        var match = parser.Matches(context);
        _output.WriteFullContext(context);

        // arrange
        match.ShouldBeTrue();
    }

    #endregion
}