using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public record SyntaxNode
{
    #region Constructors

    public SyntaxNode(Token token, IEnumerable<SyntaxNode>? children = null)
    {
        Token = token;
        Children = children ?? [];
    }

    #endregion

    #region Properties

    public IEnumerable<SyntaxNode> Children { get; }
    public Token Token { get; }

    #endregion
}