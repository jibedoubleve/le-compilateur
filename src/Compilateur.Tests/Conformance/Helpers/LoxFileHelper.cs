namespace Compilateur.Tests.Conformance.Helpers;

public static class LoxFileHelper
{
    #region Methods

    public static TestContext ReadFile(string path)
    {
        var builder = new TestContextBuilder().WithFileName(path);

        foreach (var line in File.ReadAllLines(path))
        {
            builder.AppendCodeLine(line);

            var m = LoxRegex.ExpectedOutput().Match(line);
            if (m.Success) { builder.AppendOutputLine(m.Groups[1].Value); }

            m = LoxRegex.StaticError().Match(line);
            if (m.Success)
            {
                builder.AppendStaticError(
                    m.Groups["message"].Value,
                    m.Groups["variable"].Value,
                    m.Groups["line"].Success ? int.Parse(m.Groups["line"].Value) : null
                );
            }

            m = LoxRegex.RuntimeError().Match(line);
            if (m.Success)
            {
                builder.AppendRuntimeError(
                    m.Groups[1].Value
                );
            }
        }

        return builder.ToTestContext();
    }

    #endregion
}