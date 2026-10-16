using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Runtime;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Shouldly;

namespace Compilateur.Tests.Runtime;

public class EvaluatorTest
{
    #region Methods

    private static Token NewToken(string lexeme, TokenKind kind) =>
        new()
        {
            Column = 0,
            Line = 0,
            Kind = kind,
            Value = lexeme,
            Lexeme = lexeme
        };

    [Fact]
    public void When_Print_Text_Then_Expected_Text()
    {
        // arrange
        const string output = "Some text to output";
        const string code = $"""
                            print "{output}"
                            """;
        //act
        
        // assert
        // Assert.Multiple(
        //     () => result.ShouldBeNull(),
        //     () => textWriter.ToString().ShouldBe($"{output}\r\n")
        // );
    }

    #endregion
}