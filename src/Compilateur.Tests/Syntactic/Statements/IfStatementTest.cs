using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class IfStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public IfStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public void When_If_Branch_Is_Not_A_Statement_Then_Error_Raised()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .If()
                      .BetweenParentheses(b => b.Identifier("foo"))
                      .CloseCurlyBracket()
                      .BuildParsingContext();

        // act

        var node =  ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);
        
        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldHaveSingleItem()
        );
    }

    [Fact]
    public Task When_If_Has_Else_Branch_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .If()
                      .BetweenParentheses(b =>
                          b.Identifier("foo")
                           .GreaterThan()
                           .Number(4)
                      )
                      .BetweenCurlyBracket(b =>
                          b.Identifier("foo")
                           .Equal()
                           .Number(4)
                           .Plus()
                           .Number(7)
                           .Semicolon())
                      .Else()
                      .BetweenCurlyBracket(b =>
                          b.Identifier("bar")
                           .Equal()
                           .Number(4)
                           .Divided()
                           .Number(2)
                           .Semicolon())
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_If_Branch_Is_Single_Statement_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .If().BetweenParentheses(b => b.Identifier("foo"))
                      .Print("Hello world")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_If_Branch_Is_Block_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .If()
                      .BetweenParentheses(b =>
                          b.Identifier("foo")
                           .GreaterThan()
                           .Number(4)
                      )
                      .BetweenCurlyBracket(b =>
                          b.Identifier("foo")
                           .Equal()
                           .Number(4)
                           .Plus()
                           .Number(7)
                           .Semicolon())
                      .BuildParsingContext();
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteSyntaxContext(context: context, node: node);

        // assert
        return Verify(node);
    }

    #endregion
}