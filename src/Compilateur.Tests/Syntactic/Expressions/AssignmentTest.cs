using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Expressions;

public class AssignmentTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public AssignmentTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildInvalidExpression()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .Identifier("a")
                .Equal()
                .Identifier("b")
                .Equal()
                .Number(3)
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .Identifier("foo")
                .Dot()
                .Identifier("bar")
                .Equal()
                .Number(2)
                .BuildParsingContext()
        ];
    }

    [Fact]
    public void When_Invalid_Tokens_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Equal()
                      .Number(2)
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Theory]
    [MemberData(nameof(BuildInvalidExpression))]
    public void When_Valid_Expression_Then_Syntax_Tree_Returned(ParsingContext context)
    {
        // arrange
        var parser = new ExpressionParser();
        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => context.Errors.ShouldBeEmpty(context.FormatErrors()),
            () => node.ShouldNotBeNull()
        );
    }

    [Fact]
    public void When_Var_Declared_And_Assigned_Then_Error_Raised()
    {
        // arrange
        // var myVar = (1+6) 
        var context = new TokenCollectionBuilder()
                      .Var("myVar")
                      .Equal()
                      .BetweenParentheses(b => b.Number(1)
                                                .Plus()
                                                .Number(6))
                      .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var node = parser.Parse(context);

        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => context.Errors.ShouldNotBeEmpty(context.FormatErrors()),
            () => node.ShouldBeNull()
        );
    }

    #endregion
}