using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Rules.Declarations;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Declarations;

public class FunDeclarationTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public FunDeclarationTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Fun_Declared_And_Defined_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name,
                          b => b.Identifier("a")
                                .Comma()
                                .Identifier("b")
                      ).BetweenCurlyBracket(b =>
                          b.Number(6)
                           .Plus()
                           .Number(5)
                           .Semicolon()
                           .Number(6)
                           .Multiply()
                           .Number(8)
                           .Semicolon())
                      .BuildParsingContext();

        var p = new DeclarationParser();
        _output.WriteCode(context);

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    [Fact]
    public Task When_Fun_Declared_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name,
                          b => b.Identifier("a")
                                .Comma()
                                .Identifier("b")
                      )
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        var p = new DeclarationParser();
        _output.WriteCode(context);

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    [Fact]
    public Task When_Fun_Declared_With_Param_And_Declaration_Of_Same_Name_Then_Segregation_In_AST_Exists()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name, b => b.Identifier("a"))
                      .BetweenCurlyBracket(b => b.Identifier("a")
                                                 .Semicolon())
                      .BuildParsingContext();

        var p = new DeclarationParser();
        _output.WriteCode(context);

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    #endregion
}