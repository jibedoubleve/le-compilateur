using Compilateur.Core.Syntactic;
using Shouldly;

namespace Compilateur.Tests.Helpers;

public static class NodeExtension
{
    #region Methods

    public static SyntaxNode Child(this SyntaxNode node, int index)
    {
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => node.Children.Count().ShouldBeGreaterThanOrEqualTo(index)
        );
        return node.Children.ElementAt(index);
    }

    #endregion
}