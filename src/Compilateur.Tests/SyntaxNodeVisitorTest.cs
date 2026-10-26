using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Shouldly;

namespace Compilateur.Tests;

[Trait("Syntactic", "Syntactic")]
public class SyntaxNodeVisitorTest
{
    #region Methods

    [Fact]
    public void When_Accept_Then_Returns_Visitor_Result()
    {
        // arrange
        const int val = 42;
        var visitor = new TextSyntaxNodeVisitor(val);

        var token = new Token
        {
            Column = 0,
            Line = 0,
            Kind = TokenKind.And,
            Lexeme = "",
            Value = 44
        };

        var program = new ProgramNode(token, []);

        // act & assert
        program.Accept(visitor).ShouldBe(val);
        

    }

    #endregion
}