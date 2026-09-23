using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
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
    public Task When_Valid_If_Else_Statement_Then_Expected_Tree_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Valid_If_Expression_Then_Expected_Tree_Returned()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .If().BetweenParentheses(b => b.Identifier("foo"))
                      .Print("Hello world")
                      .Semicolon()
                      .BuildParsingContext();

        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    [Fact]
    public Task When_Valid_If_Statement_Then_Expected_Tree_Returned()
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
        _output.WriteFullContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}