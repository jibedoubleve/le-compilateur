using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Declarations;

public class FunctionParserTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public FunctionParserTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildFunctionWithInvalidArguments()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("ChainedCall")
                .EmptyCall()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("DefinitionIsSemicolon")
                .BetweenCurlyBracket(b => b.Semicolon().Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("MissingSemicolonInDefinition")
                .BetweenCurlyBracket(b => b.Number(4).Plus().Number(6)
                                           .Semicolon()
                                           .Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("MissingSemicolonInDefinition")
                .BetweenCurlyBracket(b => b.Number(4).Plus().Number(6)
                                           .Number(2).Plus().Number(3)
                                           .Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("NoIdentifier")
                .BetweenParentheses(b => b.Number(1))
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("TooManyCommas")
                .BetweenParentheses(b => b.Identifier("a")
                                          .Comma()
                                          .Comma()
                                          .Identifier("b"))
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];

        yield return
        [
            new TokenCollectionBuilder()
                .Fun("TrailingComma")
                .BetweenParentheses(b => b.Identifier("a")
                                          .Comma()
                                          .Identifier("b")
                                          .Comma())
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];

        yield return
        [
            new TokenCollectionBuilder()
                .Fun("NotOnlyIdentifiers")
                .BetweenParentheses(b => b.Number(1)
                                          .Comma()
                                          .Identifier("a"))
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];
    }

    [Theory]
    [MemberData(nameof(BuildFunctionWithInvalidArguments))]
    public void When_Invalid_Arguments_Then_Error_Displayed(ParsingContext context)
    {
        // arrange
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => context.Errors.Count().ShouldBeGreaterThan(0),
            () => node.ShouldBeNull()
        );
    }

    [Fact]
    public void When_No_Bracket_In_Definition_Then_Error_Displayed()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Fun()
                      .Identifier("NoBracket")
                      .BetweenCurlyBracket()
                      .BuildParsingContext();
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.Count().ShouldBe(2)
        );
    }

    #endregion
}