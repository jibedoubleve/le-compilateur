using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Helpers;
using Shouldly;

namespace Compilateur.Tests.CrossCutting;

public class NodeFormatterTest
{
    #region Methods

    [Theory]
    [InlineData(SyntaxNodeRole.Argument)]
    [InlineData(SyntaxNodeRole.Declaration)]
    [InlineData(SyntaxNodeRole.Function)]
    [InlineData(SyntaxNodeRole.Class)]
    [InlineData(SyntaxNodeRole.Var)]
    [InlineData(SyntaxNodeRole.Call)]
    [InlineData(SyntaxNodeRole.Get)]
    [InlineData(SyntaxNodeRole.Body)]
    [InlineData(SyntaxNodeRole.Condition)]
    [InlineData(SyntaxNodeRole.Else)]
    [InlineData(SyntaxNodeRole.Then)]
    [InlineData(SyntaxNodeRole.Init)]
    [InlineData(SyntaxNodeRole.Increment)]
    public void When_Has_Role_Then_Format_Shows_Role_And_Lexeme(SyntaxNodeRole role)
    {
        // arrange
        var node = SyntaxNodeFactory.Create(role);

        // act
        var format = node.FormatTree();

        // assert
        Assert.Multiple(
            () => format.Contains(',').ShouldBeTrue($"Should contain ',' when role is {role}"),
            () => format.Contains(role.ToString(), StringComparison.InvariantCultureIgnoreCase).ShouldBeTrue(),
            () => format.Contains(SyntaxNodeFactory.Lexeme, StringComparison.InvariantCulture).ShouldBeTrue()
        );
    }

    [Fact]
    public void When_Has_Unspecified_Role_Then_Format_Only_Shows_Lexeme()
    {
        // arrange
        const SyntaxNodeRole role = SyntaxNodeRole.Unspecified;
        var node = SyntaxNodeFactory.Create(role);

        // act
        var format = node.FormatTree();

        // assert
        Assert.Multiple(
            () => format.Contains(',').ShouldBeFalse($"Should NOT contain ',' when role is {role}"),
            () => format.Contains(role.ToString(), StringComparison.InvariantCultureIgnoreCase).ShouldBeFalse(),
            () => format.Contains(SyntaxNodeFactory.Lexeme, StringComparison.InvariantCulture).ShouldBeTrue()
        );
    }


    [Fact]
    public void When_Checking_One_Of_Type_Then_Expected_Type_Is_Found()
    {
        // arrange
        var node = SyntaxNodeFactory.Create(SyntaxNodeRole.Argument);
        
        // assert
        node.IsOneOfRole(SyntaxNodeRole.Argument, SyntaxNodeRole.Body).ShouldBeTrue();
    }
    [Fact]
    public void When_Checking_One_Of_Type_Then_Expected_Type_Is_Not_Found()
    {
        // arrange
        var node = SyntaxNodeFactory.Create(SyntaxNodeRole.Argument);
        
        // assert
        node.IsOneOfRole(SyntaxNodeRole.Class, SyntaxNodeRole.Body).ShouldBeFalse();
    }
    
    [Fact]
    public void When_Checking_One_Of_Type_On_Null_Node_Then_Nothing_Is_Found()
    {
        // arrange
        SyntaxNode? node = null;
        
        // assert
        node.IsOneOfRole(SyntaxNodeRole.Class, SyntaxNodeRole.Body).ShouldBeFalse();
    }

    #endregion
}