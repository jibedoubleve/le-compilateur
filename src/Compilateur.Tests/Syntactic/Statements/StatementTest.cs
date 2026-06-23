using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Statements;
using Compilateur.Tests.Helpers;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class StatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public StatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Block_With_Var_Defined_Then_Ok()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.Var("a")
                           .Equal()
                           .Number(14)
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
    public Task When_Block_Defined_Then_Ok()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .BetweenCurlyBracket(b =>
                          b.Identifier("a")
                           .Equal()
                           .Number(14)
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