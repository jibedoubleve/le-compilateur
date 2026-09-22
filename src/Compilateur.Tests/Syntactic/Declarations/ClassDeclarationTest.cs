using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
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
                .Class("ClassDefinitionIsSemicolon",
                    b => b.Semicolon()
                          .Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Class("BallOfMud", b => b.Fun("myFunOne", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BetweenCurlyBracket(b => b.Fun("myFunTwo", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BetweenCurlyBracket(b => b.Fun("myFunThree", c => c.Number(6).Plus().Number(5).Semicolon()))
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Symbol(TokenType.Class)
                .Identifier("EmptyNoCloseBracket")
                .OpenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Symbol(TokenType.Class)
                .Identifier("NoOpenBracket")
                .Fun("MyFunOne", c => c.Number(6).Plus().Number(5))
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '{' 
        [
            new TokenCollectionBuilder()
                .Symbol(TokenType.Class)
                .Identifier("NoOpenBracketNorFunDefinition")
                .Number(6).Plus().Number(5)
                .Semicolon()
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '}'
        [
            new TokenCollectionBuilder()
                .Symbol(TokenType.Class)
                .Identifier("MissingClosingBracket")
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
                .BuildParsingContext()
        ];
        yield return // missing class name with functions
        [
            new TokenCollectionBuilder()
                .Class(null,
                    b => b.Identifier("myFunc")
                          .BetweenCurlyBracket(c
                              => c.Number(6).Plus().Number(5)
                                  .Semicolon())
                )
                .BuildParsingContext()
        ];
        yield return // missing ';'
        [
            new TokenCollectionBuilder()
                .Class("MissingSemicolon",
                    b =>
                        b.Identifier("myFunc")
                         .BetweenCurlyBracket(c
                             => c.Number(6).Plus().Number(5)))
                .BuildParsingContext()
        ];
        yield return // invalid function in the middle of definitions
        [
            new TokenCollectionBuilder()
                .Class("SomeFailingFuncDefinition",
                    b =>
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
                      .Class("MyClass",
                          b => b.Identifier("myFunc")
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
        _output.WriteFullContext(context, node);

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
        _output.WriteFullContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Class_Has_Only_One_Function_Then_Parsing_Has_No_Error()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass",
                          b => b.Identifier("myFunc")
                                .EmptyCall()
                                .BetweenCurlyBracket(c
                                    => c.Number(6).Plus().Number(5).Semicolon()
                                )
                      )
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

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
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Class_With_Hierarchy_Then_Children_Has_SyperClass()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Subclass("MyClass", "MySubClass")
                      .BetweenCurlyBracket()
                      .BuildParsingContext();
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        //assert
        return Verify(node);
    }

    [Fact]
    public void When_Invalid_Heritage_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Symbol(TokenType.Class)
                      .Identifier("MyClass")
                      .Symbol(TokenType.LessThan)
                      .Identifier("MyClass1")
                      .Symbol(TokenType.LessThan)
                      .Identifier("MyClass2")
                      .BuildParsingContext();
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        //assert

        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    #endregion
}