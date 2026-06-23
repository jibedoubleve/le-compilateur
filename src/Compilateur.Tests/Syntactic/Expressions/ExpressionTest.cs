using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class ExpressionTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ExpressionTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_Parsing_Complex_Expression_Then_Node_Is_Returned()
    {
        // arrange
        // Code: ( 1 + 2 ) / ( 3 * 4 );
        var context = new TokenCollectionBuilder().BetweenParentheses(b => b.Number(1)
                                                                            .Plus()
                                                                            .Number(2))
                                                  .Divided()
                                                  .BetweenParentheses(b => b.Number(3)
                                                                            .Multiply()
                                                                            .Number(4))
                                                  .Semicolon()
                                                  .BuildParsingContext();
        _output.WriteCode(context);

        // act
        var parser = new ExpressionParser();
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        _output.WriteSyntaxTree(node);

        // assert
        node.ShouldNotBeNull(context.FormatErrors());

        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Token.Type.ShouldBe(TokenType.Divided),
            () => node.Children.Count().ShouldBe(2),
            // (1 + 2)
            () => node.Child(0).Children.Count().ShouldBe(2),
            () => node.Child(0).Token.Type.ShouldBe(TokenType.Plus),
            () => node.Child(0).Child(0).Token.Type.ShouldBe(TokenType.Numeric),
            () => node.Child(0).Child(1).Token.Type.ShouldBe(TokenType.Numeric),
            // (3 * 4)
            () => node.Child(1).Children.Count().ShouldBe(2),
            () => node.Child(1).Token.Type.ShouldBe(TokenType.Multiply),
            () => node.Child(1).Child(0).Token.Type.ShouldBe(TokenType.Numeric),
            () => node.Child(1).Child(1).Token.Type.ShouldBe(TokenType.Numeric)
        );
    }

    [Fact]
    public void When_Parsing_Invalid_Expression_Then_Error_Written_In_List()
    {
        // arrange
        var context = new TokenCollectionBuilder().Var(
            "someVar",
            p => p.If()
        ).BuildParsingContext();

        // act
        new ExpressionParser().Parse(context);
        _output.WriteLine(context.FormatErrors());

        // assert
        context.Errors.Count().ShouldBe(1);
    }
    
    [Fact]
    public Task When_Parsing_Complex_Calculus_Then_Node_Is_Returned()
    {
        // arrange
        // (1+6) - (4*8) / 2
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b => b.Number(1)
                                                .Plus()
                                                .Number(6))
                      .Minus()
                      .BetweenParentheses(b => b.Number(4)
                                                .Multiply()
                                                .Number(8))
                      .Divided()
                      .Number(2)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new ExpressionParser();
        _output.WriteCode(context);

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxTree(node);

        // assert
        context.Errors.ShouldBeEmpty(context.FormatErrors());
        return Verify(node);
    }

    #endregion
}