using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("class")]
public record ClassStatement : StatementNode
{
    #region Constructors

    public ClassStatement(Token token, IdentifierExpression? superClass, IEnumerable<FunctionDeclarationStatement> functions)
        : base(token)
    {
        SuperClass = superClass;
        Functions = functions;
    }

    #endregion

    #region Properties

    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            var children = new List<SyntaxNode>();
            if (SuperClass is not null) { children.Add(SuperClass); }

            children.AddRange(Functions);
            return children;
        }
    }

    public IEnumerable<FunctionDeclarationStatement> Functions { get; }

    public IdentifierExpression? SuperClass { get; }

    #endregion
}