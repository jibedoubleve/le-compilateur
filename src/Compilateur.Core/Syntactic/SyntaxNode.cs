using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public record SyntaxNode
{
    #region Constructors

    private SyntaxNode(Token token, SyntaxNode?[] children, SyntaxNodeRole role)
    {
        Token = token;
        Role = role;

        Children = children.Where(c => c is not null)
                           .Select(child => child!);
    }

    #endregion

    #region Properties

    public IEnumerable<SyntaxNode> Children { get; }
    public SyntaxNodeRole Role { get; init; }
    public Token Token { get; }

    #endregion

    #region Methods

    public static SyntaxNode Argument(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Argument);

    public static SyntaxNode Call(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Call);

    public static SyntaxNode Class(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Class);

    public static SyntaxNode Declaration(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Declaration);

    public static SyntaxNode Function(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Function);

    public static SyntaxNode Get(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Get);

    public static SyntaxNode Unspecified(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Unspecified);

    public static SyntaxNode Var(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Var);

    #endregion
}

public enum SyntaxNodeRole
{
    Unspecified,
    Argument,
    Declaration,
    Function,
    Class,
    Var,
    Call,
    Get
}