using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Shouldly;

namespace Compilateur.Tests.CrossCutting;

public class ExtensionsTest
{
    #region Methods

    public static IEnumerable<object[]> GetTokenTypes()
        => Enum.GetValues<TokenType>().Select(type => (object[])[type]);

    [Theory]
    [MemberData(nameof(GetTokenTypes))]
    public void When_Checking_IsOfType_Then_Returns_True(TokenType type)
    {
        // arrange
        var node = SyntaxNodeFactory.Create(type);

        // act

        // assert
        node.IsOfType(type).ShouldBeTrue();
    }
    
    [Theory]
    [MemberData(nameof(GetTokenTypes))]
    public void When_Checking_IsOfType_On_Null_Then_Returns_False(TokenType type)
    {
        // arrange
        SyntaxNode? node = null;

        // act

        // assert
        node.IsOfType(type).ShouldBeFalse();
    }

    #endregion
}