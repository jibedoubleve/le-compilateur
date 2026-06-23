using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core;

public sealed record TokenizeResult
{
    #region Properties

    public SyntaxErrorCollection Errors { get; init; } = [];
    public bool HasErrors => Errors.Any();
    public required IReadOnlyCollection<Token> Tokens { get; init; }

    #endregion
}