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
    public SyntaxNodeRole Role { get; }
    public Token Token { get; }

    #endregion

    #region Methods

    public static SyntaxNode Argument(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Argument);

    public static SyntaxNode Body(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Body);

    public static SyntaxNode Call(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Call);

    public static SyntaxNode Class(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Class);

    public static SyntaxNode Condition(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Condition);

    public static SyntaxNode Declaration(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Declaration);

    public static SyntaxNode Else(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Else);

    public static SyntaxNode Eof() => new(
        new Token
        {
            Column = 0,
            Line = 0,
            Type = TokenType.Eof,
            Value = "",
            Lexeme = "$"
        },
        [],
        SyntaxNodeRole.Unspecified
    );

    public static SyntaxNode Function(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Function);

    public static SyntaxNode Get(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Get);

    public static SyntaxNode Increment(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Increment);

    public static SyntaxNode Init(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Init);

    public static SyntaxNode Then(SyntaxNode src)
        => new(src.Token, [.. src.Children], SyntaxNodeRole.Then);

    public static SyntaxNode Unspecified(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Unspecified);

    public static SyntaxNode Var(Token token, params SyntaxNode?[] children)
        => new(token, children, SyntaxNodeRole.Var);

    #endregion
}