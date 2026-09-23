using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
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

    public static IEnumerable<object[]> BuildInvalidFunDeclarations()
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
                .EmptyCall()
                .BetweenCurlyBracket(b => b.Semicolon().Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("MissingSemicolonInDefinition")
                .EmptyCall()
                .BetweenCurlyBracket(b => b.Number(4).Plus().Number(6)
                                           .Semicolon()
                                           .Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("MissingSemicolonInDefinition")
                .EmptyCall()
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
                .Fun("NoIdentifier")
                .BetweenParentheses(b => b.Identifier("foo")
                                          .Identifier("bar"))
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Fun("NoIdentifier")
                .BetweenParentheses(b => b.Identifier("foo")
                                          .Identifier("bar")
                                          .Identifier("baz"))
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

    [Fact]
    public void When_Fun_Body_Statement_Has_No_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Fun("foo").EmptyCall()
                      .BetweenCurlyBracket(b =>
                          b.Number(1)
                           .Number(2))
                      .BetweenCurlyBracket(b => b.Var("bar")
                                                 .Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        //assert
        Assert.Multiple(
            () => context.Errors.ShouldNotBeEmpty(),
            () => node.ShouldBeNull()
        );
    }

    [Theory]
    [MemberData(nameof(BuildInvalidFunDeclarations))]
    public void When_Fun_Declaration_Is_Invalid_Then_Error_Raised(ParsingContext context)
    {
        // arrange
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => context.Errors.Count().ShouldBeGreaterThan(0),
            () => node.ShouldBeNull()
        );
    }

    [Fact]
    public void When_Fun_Has_No_Parentheses_Then_Single_Error_Raised()
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
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.Count().ShouldBe(1)
        );
    }


    [Fact]
    public void When_Fun_Parameters_Not_Closed_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Fun("foo")
                      .OpenParenthesis()
                      .Number(1)
                      .Comma()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        //assert
        Assert.Multiple(
            () => context.Errors.ShouldNotBeEmpty(),
            () => node.ShouldBeNull()
        );
    }

    #endregion
}