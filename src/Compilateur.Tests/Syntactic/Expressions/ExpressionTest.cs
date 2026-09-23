using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Expressions;
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
    public Task When_Arithmetic_Has_Groups_Then_Expected_Tree_Returned()
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

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        context.Errors.ShouldBeEmpty(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public Task When_Arithmetic_Has_No_Groups_Then_Precedence_Is_Respected()
    {
        // arrange
        // 1+6 - 4*8 / 2
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Plus()
                      .Number(6)
                      .Minus()
                      .Number(4)
                      .Multiply()
                      .Number(8)
                      .Divided()
                      .Number(2)
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        context.Errors.ShouldBeEmpty(context.FormatErrors());
        return Verify(node);
    }

    [Fact]
    public void When_Declaration_Given_To_Expression_Parser_Then_Single_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder().Var(
            "someVar",
            p => p.If()
        ).BuildParsingContext();

        // act
        var node = new ExpressionParser().Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        context.Errors.Count().ShouldBe(1);
    }

    #endregion
}