using System.Text;

namespace Compilateur.Tests.Conformance.Helpers;

public static class PathExtensions
{
    #region Methods

    private static string GetFileName(string path)
    {
        var fileName = Path.GetFileName(path);
        var dirName = Path.GetFileName(Path.GetDirectoryName(path));
        return Path.Combine(dirName ?? ".\\", fileName);
    }

    public static TestContext ReadFile(this string path)
    {
        var code = new StringBuilder();
        var output = new StringBuilder();
        var errors = new StringBuilder();
        string? runtimeError = null;

        foreach (var line in File.ReadAllLines(path))
        {
            code.AppendLine(line);
            var m = Helpers.LoxRegex.Expect().Match(line);
            if (m.Success)
            {
                output.AppendLine(m.Groups[1].Value);
            }

            m = Helpers.LoxRegex.ErrorAt().Match(line);
            if (m.Success)
            {
                errors.AppendLine(m.Groups[1].Value);
            }

            m = Helpers.LoxRegex.RuntimeErrorAt().Match(line);
            if(runtimeError is null && m.Success)
            {
                runtimeError = m.Groups[1].Value;
            }
                
        }

        return new TestContext(
            GetFileName(path),
            code,
            output,
            errors,
            runtimeError
        );
    }

    #endregion
}