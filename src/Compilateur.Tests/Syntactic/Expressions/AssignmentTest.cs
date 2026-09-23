using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
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

    public static IEnumerable<object[]> BuildValidAssignments()
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
    public void When_Assignment_Has_No_Value_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .Equal()
                      .Semicolon()
                      .BuildParsingContext();
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);
        
        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty(context.FormatErrors())
        );
    }
    [Fact]
    public void When_Assignment_Target_Is_Group_Then_Error_Raised()
    {
        // assert
        var context = new TokenCollectionBuilder()
                      .BetweenParentheses(b => b.Identifier("foo"))
                      .Equal().Number(1)
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => context.Errors.ShouldNotBeEmpty(),
            () => node.ShouldBeNull()
        );
    }

    [Fact]
    public void When_Assignment_Target_Is_Literal_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Number(1)
                      .Equal()
                      .Number(2)
                      .Semicolon()
                      .BuildParsingContext();

        //act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        Assert.Multiple(
            () => context.Errors.ShouldNotBeEmpty(context.FormatErrors()),
            () => node.ShouldBeNull()
        );
    }

    [Fact]
    public Task When_Assignments_Are_Chained_Then_Tree_Is_Right_Associative()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .Equal()
                      .Identifier("b")
                      .Equal()
                      .Identifier("c")
                      .Equal()
                      .Number(15)
                      .Semicolon()
                      .BuildParsingContext();

        //act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Assigning_To_Property_Then_Set_Node_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Identifier("a")
                      .Dot()
                      .Identifier("b")
                      .Equal()
                      .Number(15)
                      .Semicolon()
                      .BuildParsingContext();

        //act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    #endregion
}