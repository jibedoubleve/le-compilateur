using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Runtime;

public class Evaluator : ISyntaxNodeVisitor<object?>
{
    #region Fields

    private readonly TextWriter _textWriter;

    #endregion

    #region Constructors

    public Evaluator() => _textWriter = Console.Out;
    public Evaluator(TextWriter textWriter) => _textWriter = textWriter;

    #endregion

    #region Properties

    public ErrorCollection Errors { get; } = new();

    #endregion

    #region Methods

    public object? Visit(ClassStatement node) => throw new NotImplementedException();

    public object? Visit(FunctionDeclarationStatement node) => throw new NotImplementedException();

    public object? Visit(VarDeclarationStatement node) => throw new NotImplementedException();

    public object? Visit(AssignExpression node) => throw new NotImplementedException();

    public object? Visit(BinaryExpression node) => throw new NotImplementedException();

    public object? Visit(CallExpression node) => throw new NotImplementedException();

    public object? Visit(GetExpression node) => throw new NotImplementedException();

    public object? Visit(GroupExpressionNode node) => throw new NotImplementedException();

    public object? Visit(IdentifierExpression node) => throw new NotImplementedException();

    public object? Visit(LiteralExpression node) => throw new NotImplementedException();

    public object? Visit(LogicalExpression node) => throw new NotImplementedException();

    public object? Visit(SetExpression node) => throw new NotImplementedException();

    public object? Visit(SuperExpression node) => throw new NotImplementedException();

    public object? Visit(ThisExpression node) => throw new NotImplementedException();

    public object? Visit(UnaryExpression node) => throw new NotImplementedException();

    public object? Visit(BlockStatement node) => throw new NotImplementedException();

    public object? Visit(ExpressionStatement node) => throw new NotImplementedException();

    public object? Visit(IfStatement node) => throw new NotImplementedException();

    public object? Visit(PrintStatement node)
    {
        _textWriter.WriteLine(node.Value.Token.Lexeme);
        return null;
    }

    public object? Visit(ReturnStatement node) => throw new NotImplementedException();

    public object? Visit(WhileStatement node) => throw new NotImplementedException();

    public object? Visit(ProgramNode node) => throw new NotImplementedException();

    #endregion
}