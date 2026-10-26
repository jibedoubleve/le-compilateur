using System.Text.RegularExpressions;

namespace Compilateur.Tests.Conformance.Helpers;

/// <summary>
///     Patterns matching the expectation comments embedded in the
///     Crafting Interpreters <c>.lox</c> test files. Each comment
///     describes what the interpreter must produce for the line it
///     sits on.
/// </summary>
public static partial class LoxRegex
{
    #region Methods

    /// <summary>
    ///     Expected standard output: <c>// expect: 3</c>.
    ///     Group 1 captures the exact line the program must print.
    /// </summary>
    [GeneratedRegex("// expect: (.*)")]
    public static partial Regex ExpectedOutput();

    /// <summary>
    ///     Runtime error raised while executing the line holding the
    ///     comment: <c>// expect runtime error: message</c>.
    ///     Group 1 captures the message; no token is reported.
    /// </summary>
    [GeneratedRegex("// expect runtime error: (.*)")]
    public static partial Regex RuntimeError();

    /// <summary>
    ///     Error reported on an explicit line rather than the comment's
    ///     own line: <c>// [line 3] Error at 'x': message</c>.
    ///     Groups: 1 = line number, 2 = offending token, 3 = message.
    /// </summary>
    [GeneratedRegex(@"// (?:\[line (?<line>\d+)\] )?Error at (?:'(?<variable>.+)'|end): (?<message>.*)")]
    public static partial Regex StaticError();

    #endregion
}