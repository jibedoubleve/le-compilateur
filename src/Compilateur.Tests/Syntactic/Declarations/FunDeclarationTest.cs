using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Declarations;
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
    public Task When_Declaring_Fun_With_2_Params_Then_Parsing_Returns_Expected_Node()
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

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    [Fact]
    public Task When_Declaring_Fun_With_3_Params_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name,
                          b => b.Identifier("a")
                                .Comma()
                                .Identifier("b")
                                .Comma()
                                .Identifier("c")
                      )
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
        return Verify(node);
    }

    [Fact]
    public void When_Defining_Function_With_More_Than_255_Parameter_Then_Error_Raised()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name,
                          b => {
                              for (var i = 0; i < 260; i++)
                              {
                                  b.Identifier("a")
                                   .Comma();
                              }

                              b.Identifier("lastA");
                          }
                      )
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
    }

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

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
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

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    #endregion
}