using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
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
    public Task When_Var_Declared_In_Block_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b => b.Var("foo")
                                                 .Semicolon())
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Var_Declared_Without_Semicolon_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("myVariable")
                      .BuildParsingContext();

        var parser = new DeclarationParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => matched.ShouldBeTrue(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Fact]
    public void When_Var_Has_Initialiser_Then_Initialiser_Is_Parsed()
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
        _output.WriteSyntaxContext(context, node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.ShouldBeOfType<VarDeclarationStatement>(),
            () => ((VarDeclarationStatement)node).Initialiser.ShouldBeOfType<BinaryExpression>(),
            () => ((VarDeclarationStatement)node).Token.Kind.ShouldBe(TokenKind.Identifier)
        );
    }

    [Fact]
    public Task When_Var_Has_No_Initialiser_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Var("foo")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public void When_Var_Initialiser_Is_Invalid_Then_Null_Returned()
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
        _output.WriteSyntaxContext(context, node);
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => matched.ShouldBeTrue()
        );
    }

    #endregion
}