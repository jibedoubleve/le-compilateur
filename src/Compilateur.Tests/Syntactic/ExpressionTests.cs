using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic;

public partial class ExpressionTests
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ExpressionTests(ITestOutputHelper output) => _output = output;

    #endregion

    #region Properties

    private static TokenCollectionBuilder Tokens => new();

    #endregion

    #region Methods

    [Fact]
    public void When_Parsing_Addition_Then_Node_Is_Returned()
    {
        // arrange
        var context = Tokens.Number(1)
                            .Plus()
                            .Number(2)
                            .BuildParsingContext();
        var parser = new ExpressionParser();

        // act
        var matched = parser.Matches(context);
        var node = parser.Parse(context);

        // assert
        node.ShouldNotBeNull();
        Assert.Multiple(
            () => matched.ShouldBeTrue(),
            () => node.Children.Count().ShouldBe(2)
        );
    }

    [Fact]
    public void When_Parsing_Invalid_Expression_Then_Error_Written_In_List()
    {
        // arrange
        var context = Tokens.Var(
            "someVar",
            p => p.If()
        ).BuildParsingContext();

        // act
        new ExpressionParser().Parse(context);
        _output.WriteLine(context.FormatErrors());

        // assert
        context.Errors.Count().ShouldBe(1);
    }

    #endregion
}