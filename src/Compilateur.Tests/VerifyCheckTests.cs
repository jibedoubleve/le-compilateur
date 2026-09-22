namespace Compilateur.Tests;

public class VerifyCheckTest
{
    #region Methods

    [Fact]
    public Task When_Using_Verify_Then_Configuration_Is_Good() => VerifyChecks.Run();

    #endregion
}