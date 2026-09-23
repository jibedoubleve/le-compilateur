using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Shouldly;

namespace Compilateur.Tests.CrossCutting;

public class ExtensionsTest
{
    #region Methods

    public static IEnumerable<object[]> GetTokenTypes()
        => Enum.GetValues<TokenKind>().Select(type => (object[])[type]);

    [Theory]
    [MemberData(nameof(GetTokenTypes))]
    public void When_Checking_IsOfKind_Then_Returns_True(TokenKind kind)
    {
        // arrange
        var token = new Token
        {
            Kind = kind,
            Lexeme = string.Empty,
            Column = 0,
            Line = 0,
            Value = null
        };
        var node = new TestNode(token);

        // act

        // assert
        node.IsOfKind(kind).ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(GetTokenTypes))]
    public void When_Checking_IsOfType_On_Null_Then_Returns_False(TokenKind kind)
    {
        // arrange
        SyntaxNode? node = null;

        // act

        // assert
        node.IsOfKind(kind).ShouldBeFalse();
    }

    #endregion

    private record TestNode : SyntaxNode
    {
        #region Constructors

        public TestNode(Token token) : base(token) { }

        #endregion
    }
}