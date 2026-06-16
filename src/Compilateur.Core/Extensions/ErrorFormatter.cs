using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Extensions;

public static class ErrorFormater
{
    #region Methods

    public static string FormatErrors(this ParsingContext context) => context.Errors.Format();

    public static string Format(this SyntaxErrorCollection errors)
    {
        var builder = new StringBuilder();
        builder.AppendLine("| line | col  | error                      ");
        builder.AppendLine("|------|------|----------------------------");
        foreach (var error in errors)
        {
            var line = $"| {error.Line,4} | {error.Column,4} | {error.Message} |";
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    #endregion
}