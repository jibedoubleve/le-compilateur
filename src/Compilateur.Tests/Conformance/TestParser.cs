using System.Text;

namespace Compilateur.Tests.Conformance;

public class TestParser
{
    #region Fields

    private readonly string _path;

    #endregion

    #region Constructors

    public TestParser(string path) => _path = path;

    #endregion

    #region Methods

    public (StringBuilder Code, StringBuilder Output, StringBuilder Errors) ReadFile()
    {
        var code = new StringBuilder();
        var output = new StringBuilder();
        var errors = new StringBuilder();

        foreach (var line in File.ReadAllLines(_path))
        {
            code.AppendLine(line);
            var m = LoxExtractor.Expect().Match(line);
            if (m.Success)
            {
                output.AppendLine(m.Groups[1].Value);
            }
            
            m = LoxExtractor.ErrorAt().Match(line);
            if (m.Success)
            {
                errors.AppendLine(m.Groups[1].Value);
            }
        }

        return (code, output, errors);
    }

    #endregion
}