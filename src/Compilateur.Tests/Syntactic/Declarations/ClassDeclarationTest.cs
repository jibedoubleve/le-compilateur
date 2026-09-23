using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
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
                .BetweenCurlyBracket(b => b.Semicolon()
                                           .Semicolon())
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Class("BallOfMud")
                .BetweenCurlyBracket(b =>
                    b.Fun("myFunOne").BetweenCurlyBracket(c => c.Number(6).Plus().Number(5).Semicolon())
                     .Fun("myFunTwo").BetweenCurlyBracket(c => c.Number(6).Plus().Number(5).Semicolon())
                     .Fun("myFunThree").BetweenCurlyBracket(c => c.Number(6).Plus().Number(5).Semicolon()))
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Symbol(TokenKind.Class)
                .Identifier("EmptyNoCloseBracket")
                .OpenCurlyBracket()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Symbol(TokenKind.Class)
                .Identifier("NoOpenBracket")
                .Fun("MyFunOne")
                .BetweenCurlyBracket(c => c.Number(6).Plus().Number(5))
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '{' 
        [
            new TokenCollectionBuilder()
                .Symbol(TokenKind.Class)
                .Identifier("NoOpenBracketNorFunDefinition")
                .Number(6).Plus().Number(5)
                .Semicolon()
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];
        yield return // missing '}'
        [
            new TokenCollectionBuilder()
                .Symbol(TokenKind.Class)
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
                .Class(null)
                .BetweenCurlyBracket(b => b.Identifier("myFunc")
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
    public Task When_Class_Has_Three_Methods_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket(b => b.Identifier("myFunc")
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
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Class_Body_Is_Empty_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Class_Has_One_Method_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("MyClass")
                      .BetweenCurlyBracket(b => b.Identifier("myFunc")
                                                 .EmptyCall()
                                                 .BetweenCurlyBracket(c
                                                     => c.Number(6).Plus().Number(5).Semicolon()
                                                 )
                      )
                      .BuildParsingContext();

        // act
        var parser = new DeclarationParser();
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        return Verify(node);
    }

    [Theory]
    [MemberData(nameof(BuildInvalidClassTokens))]
    public void When_Class_Is_Invalid_Then_Error_Raised(ParsingContext context)
    {
        // arrange
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Class_Has_Superclass_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Subclass("MyClass", "MySubClass")
                      .BetweenCurlyBracket()
                      .BuildParsingContext();
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        //assert
        return Verify(node);
    }

    [Fact]
    public void When_Class_Has_Two_Superclasses_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Symbol(TokenKind.Class)
                      .Identifier("MyClass")
                      .Symbol(TokenKind.LessThan)
                      .Identifier("MyClass1")
                      .Symbol(TokenKind.LessThan)
                      .Identifier("MyClass2")
                      .BuildParsingContext();
        var parser = new DeclarationParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        //assert

        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public Task When_Class_Followed_By_Statement_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("Foo")
                      .BetweenCurlyBracket(b => b.Identifier("bar")
                                                 .EmptyCall()
                                                 .BetweenCurlyBracket(c => c.Number(1)
                                                                            .Plus()
                                                                            .Number(2)
                                                                            .Semicolon()))
                      .Print("Hello world")
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // arrange
        return Verify(node);
    }

    [Fact]
    public Task When_Empty_Class_Followed_By_Statement_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Class("Foo")
                      .BetweenCurlyBracket()
                      .Print("Hello world")
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // arrange
        return Verify(node);
    }

    #endregion
}