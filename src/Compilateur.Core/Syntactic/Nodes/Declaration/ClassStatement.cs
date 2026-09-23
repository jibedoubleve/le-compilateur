using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("class")]
public sealed class ClassStatement : StatementNode
{
    #region Constructors

    public ClassStatement(
        Token token, IdentifierExpression? superClass, IEnumerable<FunctionDeclarationStatement> functions)
        : base(token)
    {
        SuperClass = superClass;
        Functions = [.. functions];
    }

    #endregion

    #region Properties

    public IReadOnlyList<FunctionDeclarationStatement> Functions { get; }

    public IdentifierExpression? SuperClass { get; }

    #endregion
}