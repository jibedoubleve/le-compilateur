using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical;

public sealed record TokenizeResult
{
    #region Properties

    public CompilationErrorCollection Errors { get; init; } = [];
    public bool HasErrors => Errors.Any();
    public required IReadOnlyCollection<Token> Tokens { get; init; }

    #endregion
}