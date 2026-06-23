using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Helpers;

public static class ParsingContextExtension
{
    #region Methods

    /// <summary>
    ///     Validates that the current token is a semicolon terminating a statement.
    ///     If the check fails, an error is recorded in the parsing context.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="node">The node to return unchanged if validation succeeds.</param>
    /// <returns><paramref name="node" /> if the current token is a semicolon; otherwise <c>null</c>.</returns>
    public static SyntaxNode? ValidateFinalSemicolon(this ParsingContext context, SyntaxNode? node)
    {
        if (context.Cursor.IsPeekOfType(TokenType.Semicolon))
        {
            context.Cursor.Consume(); // Consume the ';'
            return node;
        }

        context.AddError($"Expected ';' but found '{context.Cursor.Peek().Lexeme}'.");
        return null;
    }

    #endregion
}