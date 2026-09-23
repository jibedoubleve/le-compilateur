using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Declarations;

public class VarDeclarationTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public VarDeclarationTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Malformed_Var_Declared_And_Assigned_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        // Build malformed "var myVariable = !;"
        const string name = "myVariable";
        var context = new TokenCollectionBuilder()
                      .Var(name, b => b.Bang())
                      .Semicolon()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        _output.WriteFullContext(context, node);
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => matched.ShouldBeTrue()
        );
    }

    [Fact]
    public void When_Var_Declared_And_Assigned_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        const string name = "myVariable";
        var context = new TokenCollectionBuilder()
                      .Var(name,
                          b => {
                              b.Number(1);
                              b.Plus();
                              b.Number(2);
                          })
                      .Semicolon()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Token.Kind.ShouldBe(TokenKind.Identifier),
            () => node.Children.Count().ShouldBe(1)
        );
    }

    [Fact]
    public Task When_Var_Declared_In_Block_Then_Expected_Returns_Expected_Node()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b => b.Var("foo")
                                                 .Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Var_Declared_Then_Expected_Returns_Expected_Node()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("foo")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Var_Declared_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("myVariable")
                      .Semicolon()
                      .BuildParsingContext();

        var p = new DeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        node.ShouldNotBeNull();
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Token.Kind.ShouldBe(TokenKind.Identifier)
        );
    }

    [Fact]
    public void When_Var_Declared_Without_Semicolon_Then_Parsing_Returns_Error()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("myVariable")
                      .BuildParsingContext();

        var parser = new DeclarationParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => matched.ShouldBeTrue(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    #endregion
}