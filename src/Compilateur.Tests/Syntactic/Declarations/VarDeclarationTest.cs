using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Declarations;
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

    public static IEnumerable<object[]> Build_Var_Expressions()
    {
        const string name = "myVariable";
        yield return
        [
            new TokenCollectionBuilder()
                .Var(name, b => b.Bang())
                .BuildParsingContext()
        ];
    }

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
            () => node.Token.Type.ShouldBe(TokenType.Identifier),
            () => node.Children.Count().ShouldBe(1)
        );
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
            () => node.Token.Type.ShouldBe(TokenType.Identifier)
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

    [Fact]
    public void When_Var_Declared_In_Program_Then_Parsing_Returns_Expected_Node()
    {
        
    }
    [Fact]
    public void When_Var_Declared_In_Block_Then_Parsing_Returns_Expected_Node()
    {
        
    }
    #endregion
}