using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Compilateur.Tests.Integration;

//Todo: send it to the parser tests
public class FrontEndTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public FrontEndTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Fact]
    public Task When_Parsing_Full_Program_Then_Expected_Tree_Returned()
    {
        // arrange
        const string code = """
                            // One line comments
                            var android = 42;

                            var pi = 3.14;
                            var name = "hello world";
                            var flag = true;
                            var nothing = nil;

                            /* multiline comment on one line */
                            if (android >= 10 and flag) {
                                print android + pi;
                            } else {
                                print name;
                            }
                            /* Multiline comments
                             * on multiple lines
                             */
                            fun greet(who) {
                                return "hi " + who;
                            }

                            var result = greet(name);
                            print result != "bye";
                            """;

        // act
        // -- lexer
        var nodes = new Scanner().Tokenize(code);
        // -- parser
        var cursor = new TokenCursor(nodes.Tokens);
        var context = new ParsingContext(cursor, nodes.Errors);
        var node = ProgramParser.Parse(context);

        _output.WriteSyntaxContext(context, node);

        // assert
        return Verify(node);
    }

    #endregion
}