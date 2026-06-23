using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic;

public record SyntaxNode
{
    #region Constructors

    public SyntaxNode(Token token, params SyntaxNode?[] children)
    {
        Token = token;
        
        Children = children.Where(c => c is not null)
                           .Select(child => child!);
    }

    #endregion

    #region Properties

    public IEnumerable<SyntaxNode> Children { get; }
    public Token Token { get; }

    #endregion
}