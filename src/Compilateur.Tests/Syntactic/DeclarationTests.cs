using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic.Rules.Declarations;
using Compilateur.Tests.Helpers;
using Shouldly;

namespace Compilateur.Tests.Syntactic;

public class DeclarationTests
{
    #region Properties

    private static TokenCollectionBuilder Tokens => new();

    #endregion

    #region Methods

    [Fact]
    public void When_Var_Declared_And_Asigned_Token_Then_Parsing_Returns_Expected_Node()
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
                            .BuildParsingContext();

        var p = new VarDeclarationParser();

        // act
        var matched = p.Matches(context);
        var node = p.Parse(context);

        // assert
        node.ShouldNotBeNull();
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