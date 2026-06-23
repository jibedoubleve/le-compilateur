using Compilateur.Core.Extensions;
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

    [Fact]
    public Task When_Complex_Expression_Parsed_Then_Valid_Node_Returned()
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