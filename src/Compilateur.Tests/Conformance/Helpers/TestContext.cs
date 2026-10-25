using System.Text;

namespace Compilateur.Tests.Conformance.Helpers;

public record TestContext(
    string FileName,
    StringBuilder Code,
    StringBuilder Output,
    StringBuilder Errors,
    string? RuntimeError
);