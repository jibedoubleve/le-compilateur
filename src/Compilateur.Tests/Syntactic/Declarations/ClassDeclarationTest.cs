using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Declarations;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Declarations;

public class ClassDeclarationTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ClassDeclarationTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildInvalidClassTokens()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .Class("ClassDefinitionIsSemicolon")
                .OpenCurlyBracket()
                .Semicolon()
                .Semicolon()
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Class("BallOfMud")
                .BetweenCurlyBracket(b => b.Fun("myFunOne", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BetweenCurlyBracket(b => b.Fun("myFunTwo", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BetweenCurlyBracket(b => b.Fun("myFunThree", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Class("EmptyNoCloseBracket")
                .OpenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Class("NoOpenBracket")
                .Fun("MyFunOne", c => c.Number(6).Plus().Number(5))
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '{' 
        [
            new TokenCollectionBuilder()
                .Class("NoOpenBracketNorFunDefinition")
                .Number(6).Plus().Number(5)
                .Semicolon()
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '}'
        [
            new TokenCollectionBuilder()
                .Class("MissingClosingBracket")
                .OpenCurlyBracket()
                .Identifier("myFunc")
                .BetweenCurlyBracket(b =>
                    b.Number(6).Plus().Number(5)
                     .Semicolon())
                .BuildParsingContext()
        ];
        yield return // missing class name without functions
        [
            new TokenCollectionBuilder()
                .Class(null)
                .BetweenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing class name with functions
        [
            new TokenCollectionBuilder()
                .Class(null)
                .BetweenCurlyBracket(b
                    => b.Identifier("myFunc")
                        .BetweenCurlyBracket(c
                            => c.Number(6).Plus().Number(5)
                                .Semicolon())
                )
                .BuildParsingContext()
        ];
        yield return // missing ';'
        [
            new TokenCollectionBuilder()
                .Class("MissingSemicolon")
                .BetweenCurlyBracket(b =>
                    b.Identifier("myFunc")
                     .BetweenCurlyBracket(c
                         => c.Number(6).Plus().Number(5)))
                .BuildParsingContext()
        ];
        yield return // invalid function in the middle of definitions
        [
            new TokenCollectionBuilder()
                .Class("SomeFailingFuncDefinition")
                .BetweenCurlyBracket(b =>
                    b.Identifier("myFunc")
                     .EmptyCall()
                     .BetweenCurlyBracket(c
                         => c.Number(6).Plus().Number(5).Semicolon()
                     )
                     .Identifier("myFuncTwo")
                     .EmptyCall()
                     .BetweenCurlyBracket(c
                         => c.Number(6).Plus().Number(5)
                     )
                     .Identifier("myFuncThree")
                     .EmptyCall()
                     .BetweenCurlyBracket(c
                         => c.Number(6).Plus().Number(5).Semicolon()
                     ))
                .BuildParsingContext()
        ];
    }

    [Fact]
    public Task When_Class_Has_3_Functions_Then_Parsing_Has_No_Error()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket(b
                          => b.Identifier("myFunc")
                              .EmptyCall()
                              .BetweenCurlyBracket(c
                                  => c.Number(6).Plus().Number(5).Semicolon()
                              )
                              .Identifier("myOtherFunc")
                              .EmptyCall()
                              .BetweenCurlyBracket(d
                                  => d.Number(9).Divided().Number(8).Semicolon()
                              )
                              .Identifier("myThirdFunc")
                              .EmptyCall()
                              .BetweenCurlyBracket(d
                                  => d.Number(1).Multiply().Number(2).Semicolon()
                              )
                      )
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);

        _output.WriteCode(context);
        _output.WriteSyntaxTree(node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Class_Has_Empty_Definition_Then_Parsing_Has_No_Error()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);

        _output.WriteCode(context);
        _output.WriteSyntaxTree(node);
        _output.WriteLine(context.FormatErrors());

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Class_Has_Only_One_Function_Then_Parsing_Has_No_Error()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket(b
                          => b.Identifier("myFunc")
                              .EmptyCall()
                              .BetweenCurlyBracket(c
                                  => c.Number(6).Plus().Number(5).Semicolon()
                              )
                      )
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);

        _output.WriteCode(context);
        _output.WriteSyntaxTree(node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildInvalidClassTokens))]
    public void When_Class_Is_Invalid_Then_Parsing_Returns_Error(ParsingContext context)
    {
        // arrange
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);

        _output.WriteCode(context);
        _output.WriteSyntaxTree(node);
        _output.WriteLine(context.FormatErrors());

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    #endregion
}