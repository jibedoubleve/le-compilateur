namespace Compilateur.Tests.Conformance.Helpers;

public record TestContext(
    string FileName,
    string Code,
    string Output,
    IEnumerable<string> RuntimeErrors,
    IEnumerable<StaticError> StaticErrors
);