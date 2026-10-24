using System.Text;

namespace Compilateur.Tests.Conformance;

public static class PathExtensions
{
    #region Methods

    public static TestContext ReadFile(
        this string path)
    {
        var code = new StringBuilder();
        var output = new StringBuilder();
        var errors = new StringBuilder();
        string? runtimeError = null;

        foreach (var line in File.ReadAllLines(path))
        {
            code.AppendLine(line);
            var m = LoxRegex.Expect().Match(line);
            if (m.Success)
            {
                output.AppendLine(m.Groups[1].Value);
            }

            m = LoxRegex.ErrorAt().Match(line);
            if (m.Success)
            {
                errors.AppendLine(m.Groups[1].Value);
            }

            m = LoxRegex.RuntimeErrorAt().Match(line);
            runtimeError = m.Success
                ? m.Groups[1].Value
                : null;
        }

        return new TestContext(
            Path.GetFileName(path),
            code,
            output,
            errors,
            runtimeError
        );
    }


    #endregion
}