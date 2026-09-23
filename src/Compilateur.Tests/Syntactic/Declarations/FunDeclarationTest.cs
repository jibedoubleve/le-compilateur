using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Parsers;
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
    public Task When_Fun_Has_Two_Parameters_Then_Expected_Tree_Returned()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name)
                      .BetweenParentheses(b => b.Identifier("a")
                                                 .Comma()
                                                 .Identifier("b")
                      )
                      .BetweenCurlyBracket()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    [Fact]
    public Task When_Fun_Has_Three_Parameters_Then_Expected_Tree_Returned()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name)
                      .BetweenParentheses(b => b.Identifier("a")
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
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
        return Verify(node);
    }

    [Fact]
    public void When_Fun_Has_More_Than_255_Parameters_Then_Error_Raised()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name)
                      .BetweenParentheses(b => {
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
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
    }

    [Fact]
    public Task When_Fun_Has_Parameters_And_Body_Then_Expected_Tree_Returned()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name)
                      .BetweenParentheses(b => b.Identifier("a")
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

        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(context.FormatErrors()),
            () => matched.ShouldBeTrue()
        );
        return Verify(node);
    }

    [Fact]
    public Task When_Body_Uses_Parameter_Name_Then_Parameter_And_Usage_Are_Distinct_Nodes()
    {
        // arrange
        const string name = "myFunc";
        var context = new TokenCollectionBuilder()
                      .Fun(name)
                      .BetweenParentheses(b => b.Identifier("a"))
                      .BetweenCurlyBracket(b => b.Identifier("a")
                                                 .Semicolon())
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        matched.ShouldBeTrue();
        return Verify(node);
    }

    #endregion
}