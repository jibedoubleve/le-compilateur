using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Semantic;

public class Binder : ISyntaxNodeVisitor
{
    #region Fields

    public readonly Dictionary<SyntaxNode, uint> DistanceMap = new(ReferenceEqualityComparer.Instance);
    public readonly Stack<Scope> Scopes = new();

    #endregion

    public void Visit(ClassStatement node) => throw new NotImplementedException();

    public void Visit(FunctionDeclarationStatement node) => throw new NotImplementedException();

    public void Visit(VarDeclarationStatement node) => throw new NotImplementedException();

    public void Visit(AssignExpression node) => throw new NotImplementedException();

    public void Visit(BinaryExpression node) => throw new NotImplementedException();

    public void Visit(CallExpression node) => throw new NotImplementedException();

    public void Visit(GetExpression node) => throw new NotImplementedException();

    public void Visit(GroupExpressionNode node) => throw new NotImplementedException();

    public void Visit(IdentifierExpression node) => throw new NotImplementedException();

    public void Visit(LiteralExpression node) => throw new NotImplementedException();

    public void Visit(LogicalExpression node) => throw new NotImplementedException();

    public void Visit(SetExpression node) => throw new NotImplementedException();

    public void Visit(SuperExpression node) => throw new NotImplementedException();

    public void Visit(ThisExpression node) => throw new NotImplementedException();

    public void Visit(UnaryExpression node) => throw new NotImplementedException();

    public void Visit(BlockStatement node) => throw new NotImplementedException();

    public void Visit(ExpressionStatement node) => throw new NotImplementedException();

    public void Visit(IfStatement node) => throw new NotImplementedException();

    public void Visit(PrintStatement node) => throw new NotImplementedException();

    public void Visit(ReturnStatement node) => throw new NotImplementedException();

    public void Visit(WhileStatement node) => throw new NotImplementedException();

    public void Visit(ParameterNode node) => throw new NotImplementedException();

    public void Visit(ProgramNode node) => throw new NotImplementedException();
}