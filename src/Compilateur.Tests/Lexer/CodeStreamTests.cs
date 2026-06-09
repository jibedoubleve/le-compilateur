using Compilateur.Core.Lexical;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Lexer;

public class CodeCursorTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public CodeCursorTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    [Theory]
    [InlineData("", 0)]
    [InlineData("a", 1)]
    [InlineData("aa", 2)]
    [InlineData("aaa", 3)]
    public void When_Code_is_Empty_Then_Is_Eof(string code, int index)
    {
        var stream = new CodeCursor(code);
        for (var i = 0; i < index; i++)
        {
            stream.IsAtEnd.ShouldBeFalse();
            stream.Consume();
        }

        stream.IsAtEnd.ShouldBeTrue();
    }

    [Theory]
    [InlineData("a\n\rb")]
    [InlineData("a\nb")]
    [InlineData("a\rb")]
    public void When_Code_New_Line_Then_Jump_NewLine(string code)
    {
        // ARRANGE
        var stream = new CodeCursor(code);

        // ACT
        var char1 = stream.Consume();
        var newline = stream.Consume();
        var char2 = stream.Consume();

        // ASSERT
        Assert.Multiple(
            () => char1.Line.ShouldBe(1),
            () => char1.Column.ShouldBe(1),
            //---
            () => newline.Line.ShouldBe(1),
            () => newline.Column.ShouldBe(2),
            //---
            () => char2.Line.ShouldBe(2),
            () => char2.Column.ShouldBe(1)
        );
    }

    [Fact]
    public void When_NewLine_Then_Line_Number_Is_Incremented()
    {
        // ARRANGE
        const string code = """
                            1
                            2
                            3
                            4
                            """;
        var stream = new CodeCursor(code);

        // ACT
        var line1 = stream.Consume();
        var nl1 = stream.Consume();
        var line2 = stream.Consume();
        var nl2 = stream.Consume();
        var line3 = stream.Consume();
        var nl3 = stream.Consume();
        var line4 = stream.Consume();
        var nl4 = stream.Consume();

        // ASSERT
        Assert.Multiple(
            // line1
            () => line1.Line.ShouldBe(1),
            () => line1.Column.ShouldBe(1),
            
            () => nl1.Line.ShouldBe(1),
            () => nl1.Column.ShouldBe(2),
            // line2
            () => line2.Line.ShouldBe(2),
            () => line2.Column.ShouldBe(1),
            
            () => nl2.Line.ShouldBe(2),
            () => nl2.Column.ShouldBe(2),
            // line3
            () => line3.Line.ShouldBe(3),
            () => line3.Column.ShouldBe(1),
            
            () => nl3.Line.ShouldBe(3),
            () => nl3.Column.ShouldBe(2),
            // line4
            () => line4.Line.ShouldBe(4),
            () => line4.Column.ShouldBe(1)
        );
    }

    [Fact]
    public void When_Peek_Then_No_Index_Update()
    {
        const string code = "ab";
        var stream = new CodeCursor(code);

        for (var i = 0; i < 5; i++)
            Assert.Multiple(
                () => stream.PeekNext()!.IsEmpty.ShouldBeFalse(),
                () => stream.PeekNext()!.Char.ShouldBe('b')
            );
    }

    #endregion
}