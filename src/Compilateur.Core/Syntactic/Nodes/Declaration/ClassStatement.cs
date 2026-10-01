using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Nodes.Declaration;

[Description("class")]
public sealed partial class ClassStatement(
    Token token,
    IdentifierExpression? superClass,
    IEnumerable<FunctionDeclarationStatement> functions)
    : StatementNode(token)
{
    #region Properties

    public IReadOnlyList<FunctionDeclarationStatement> Functions { get; } = [.. functions];

    public IdentifierExpression? SuperClass { get; } = superClass;

    #endregion
}