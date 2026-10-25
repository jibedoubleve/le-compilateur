using System.Text;

namespace Compilateur.Tests.Conformance.Helpers;

public class TestContextBuilder
{
    #region Fields

    private readonly StringBuilder _code = new();
    private readonly StringBuilder _output = new();

    private string _path = "";
    private readonly List<string> _runtimeErrors = [];
    private readonly List<StaticError> _staticErrors = [];

    #endregion

    #region Methods

    private static string GetFileName(string path)
    {
        var fileName = Path.GetFileName(path);
        var dirName = Path.GetFileName(Path.GetDirectoryName(path));
        return Path.Combine(dirName ?? ".\\", fileName);
    }

    public TestContextBuilder AppendCodeLine(string line)
    {
        _code.AppendLine(line);
        return this;
    }

    public TestContextBuilder AppendOutputLine(string line)
    {
        _output.AppendLine(line);
        return this;
    }

    public TestContextBuilder AppendRuntimeError(string error)
    {
        _runtimeErrors.Add(error);
        return this;
    }

    public TestContextBuilder AppendStaticError(string error, string variable, int? line)
    {
        _staticErrors.Add(new StaticError(error, variable, line));
        return this;
    }

    public TestContext ToTestContext() =>
        new(
            _path,
            _code.ToString().TrimEnd(),
            _output.ToString(),
            _runtimeErrors,
            _staticErrors
        );

    public TestContextBuilder WithFileName(string path)
    {
        _path = GetFileName(path);
        return this;
    }

    #endregion
}