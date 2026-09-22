using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Compilateur.Tests.E2E;

public class FrontEndTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public FrontEndTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    private Scanner CreateScanner()
    {
        var sc = new ServiceCollection();
        var sp = sc.AddLogging(b => b.AddXunit(_output, LogLevel.Debug)
                                     .SetMinimumLevel(LogLevel.Debug))
                   .AddLexicalLayer()
                   .BuildServiceProvider();
        return sp.GetRequiredService<Scanner>();
    }

    [Fact]
    public Task When_Parsing_Code_Then_Expected_Tree_Created()
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
        var nodes = CreateScanner().Tokenize(code);
        // -- parser
        var cursor = new TokenCursor(nodes.Tokens);
        var context = new ParsingContext(cursor, nodes.Errors);
        var node = ProgramParser.Parse(context);

        _output.WriteFullContext(context, node);
        
        // assert
        return Verify(node);
    }

    #endregion
}