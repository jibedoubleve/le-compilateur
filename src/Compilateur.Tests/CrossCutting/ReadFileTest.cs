using Compilateur.Tests.Conformance.Helpers;
using Shouldly;

namespace Compilateur.Tests.CrossCutting;

[Trait("CrossCutting", "CrossCutting")]
public class ReadFileTest
{
    #region Methods

    [Fact]
    public void When_read_file_with_runtime_error_Then_correct_TextContext_returned()
    {
        // arrange
        const string path = "../../../Conformance/Lox/field\\set_evaluation_order.lox";

        // act
        var context = LoxFileHelper.ReadFile(path);

        // assert
        context.RuntimeErrors.ShouldNotBeNull();
    }

    #endregion
}