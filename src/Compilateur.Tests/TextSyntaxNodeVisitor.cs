using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Tests;

public class TextSyntaxNodeVisitor : ISyntaxNodeVisitor<int>
{
    #region Fields

    private readonly int _value;

    #endregion

    #region Constructors

    public TextSyntaxNodeVisitor(int value) => _value = value;

    #endregion

    #region Methods

    public int Visit(ClassDeclarationStatement node) => _value;

    public int Visit(FunctionDeclarationStatement node) => _value;

    public int Visit(VarDeclarationStatement node) => _value;

    public int Visit(AssignExpression node) => _value;

    public int Visit(BinaryExpression node) => _value;

    public int Visit(CallExpression node) => _value;

    public int Visit(GetExpression node) => _value;

    public int Visit(GroupExpressionNode node) => _value;

    public int Visit(IdentifierExpression node) => _value;

    public int Visit(LiteralExpression node) => _value;

    public int Visit(LogicalExpression node) => _value;

    public int Visit(SetExpression node) => _value;

    public int Visit(SuperExpression node) => _value;

    public int Visit(ThisExpression node) => _value;

    public int Visit(UnaryExpression node) => _value;

    public int Visit(BlockStatement node) => _value;

    public int Visit(ExpressionStatement node) => _value;

    public int Visit(IfStatement node) => _value;

    public int Visit(PrintStatement node) => _value;

    public int Visit(ReturnStatement node) => _value;

    public int Visit(WhileStatement node) => _value;

    public int Visit(ProgramNode node) => 42;

    #endregion
}