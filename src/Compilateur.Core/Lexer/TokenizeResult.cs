using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Lexer;

public sealed record TokenizeResult
{
    #region Properties

    public SyntaxErrorCollection Errors { get; init; } = [];
    public bool HasErrors => Errors.Any();
    public required IReadOnlyCollection<Token> Tokens { get; init; }

    #endregion
}