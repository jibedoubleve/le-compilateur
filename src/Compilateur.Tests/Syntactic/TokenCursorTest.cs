using Compilateur.Core.Syntactic;
using Compilateur.Tests.Helpers;
using Shouldly;

namespace Compilateur.Tests.Syntactic;

public class TokenCursorTest
{
    #region Methods

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void When_Cursor_At_End_Then_IsAtEnd_Is_True(int length)
    {
        var cursor = new TokenCursor(
            TokenFactory.BuildCollection(length)
        );

        for (var i = 0; i < length; i++) cursor.Consume();

        cursor.IsAtEnd.ShouldBeTrue();
    }

    #endregion
}