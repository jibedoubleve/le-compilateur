using Compilateur.Core.Syntactic.Rules;
using Compilateur.Tests.Helpers;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic;

public class ProgramParserTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public ProgramParserTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Parse_Then_Tree()
    {
        // arrange
        var context = new TokenCollectionBuilder()
                      .Print("hello_world")
                      .Semicolon()
                      .BuildParsingContext();
        var parser = new ProgramParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);
        
        // assert
        return Verify(context);
    }

    #endregion
}