using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Rules.Declarations;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic;

public class DeclarationTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public DeclarationTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Properties

    private static TokenCollectionBuilder Tokens => new();

    #endregion

    #region Methods

    public static IEnumerable<object[]> Build_Var_Expressions()
    {
        const string name = "myVariable";
        yield return
        [
            Tokens.Var(name, b => b.Bang())
                  .BuildParsingContext()
        ];
    }
    [Fact]
    public void When_Malformed_Var_Declared_And_Assigned_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        // Build malformed "var myVariable = !;"
        const string name = "myVariable";
        var context = Tokens.Var(name, b => b.Bang())
                            .BuildParsingContext();

        var p = new VarDeclarationParser();
        _output.WriteCode(context);

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => matched.ShouldBeTrue()
        );
    }

    [Fact]
    public void When_Var_Declared_And_Assigned_Token_Then_Parsing_Returns_Expected_Node()
    {
        // arrange
        // var myVariable = 1 + 2;
        const string name = "myVariable";
        var context = Tokens.Var(name,
                                b => {
                                    b.Number(1);
                                    b.Plus();
                                    b.Number(2);
                                })
                            .Semicolon()
                            .BuildParsingContext();

        var p = new VarDeclarationParser();
        _output.WriteCode(context);

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        _output.WriteSyntaxTree(node);
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
        // var myVariable;
        var context = Tokens.Var("myVariable")
                            .BuildParsingContext();

        var p = new VarDeclarationParser();

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

    #endregion
}