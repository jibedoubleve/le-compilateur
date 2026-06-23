using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;

namespace Compilateur.Tests.CrossCutting;

public static class SyntaxNodeFactory
{
    #region Fields

    public const string Lexeme = "Some_Lexeme";

    #endregion

    #region Methods

    private static Token CreateToken(TokenType? type = null) =>
        new()
        {
            Column = 0,
            Lexeme = Lexeme,
            Line = 0,
            Type = type ?? TokenType.Class,
            Value = ""
        };

    public static SyntaxNode Create(TokenType type) => SyntaxNode.Unspecified(CreateToken(type));

    public static SyntaxNode Create(SyntaxNodeRole role)
    {
        var token = CreateToken();
        return role switch
        {
            SyntaxNodeRole.Unspecified => SyntaxNode.Unspecified(token),
            SyntaxNodeRole.Argument    => SyntaxNode.Argument(token),
            SyntaxNodeRole.Declaration => SyntaxNode.Declaration(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Function    => SyntaxNode.Function(token),
            SyntaxNodeRole.Class       => SyntaxNode.Class(token),
            SyntaxNodeRole.Var         => SyntaxNode.Var(token),
            SyntaxNodeRole.Call        => SyntaxNode.Call(token),
            SyntaxNodeRole.Get         => SyntaxNode.Get(token),
            SyntaxNodeRole.Body        => SyntaxNode.Body(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Condition   => SyntaxNode.Condition(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Else        => SyntaxNode.Else(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Then        => SyntaxNode.Then(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Init        => SyntaxNode.Init(SyntaxNode.Unspecified(token)),
            SyntaxNodeRole.Increment   => SyntaxNode.Increment(SyntaxNode.Unspecified(token)),
            _                          => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    #endregion
}