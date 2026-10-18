using System.Diagnostics;
using System.Globalization;
using Compilateur.Core.Errors;
using Compilateur.Core.Runtime.Extensions;
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
    private readonly Environment _env = new();

    #endregion

    #region Constructors

    public Evaluator(ErrorCollection errors)
    {
        Errors = errors;
        _textWriter = Console.Out;
    }

    public Evaluator(ErrorCollection errors, TextWriter textWriter)
    {
        Errors = errors;
        _textWriter = textWriter;
    }

    #endregion

    #region Properties

    public ErrorCollection Errors { get; }

    #endregion

    #region Methods

    private static string Stringify(object? obj) =>
        obj switch
        {
            bool b   => b.ToString().ToLower(),
            null     => "nil",
            double d => d.ToString(CultureInfo.InvariantCulture),
            _        => obj.ToString() ?? string.Empty
        };

    public object? Visit(ClassStatement node) => throw new NotImplementedException();

    public object? Visit(FunctionDeclarationStatement node) => throw new NotImplementedException();

    public object? Visit(VarDeclarationStatement node)
    {
        var value = node.Initialiser?.Accept(this);
        var name = node.Token.Lexeme;

        _env.Current.Define(name, value);
        return null;
    }

    public object? Visit(AssignExpression node) => throw new NotImplementedException();

    public object? Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        if (left?.GetType() != right?.GetType())
        {
            var msg = node.Token.Lexeme == "+"
                ? "Operands must be two numbers or two strings."
                : "Operands must be numbers.";

            Errors.Add(node.Token, msg);
            throw new RuntimeException(Errors);
        }

        return (node.Token.Lexeme, left, right) switch
        {
            ("+", double x, double y) => x + y,
            ("+", string x, string y) => x + y,
            ("-", double x, double y) => x - y,
            ("*", double x, double y) => x * y,
            ("/", double x, double y) => x / y,
            _                         => Fail()
        };

        object Fail()
        {
            Errors.Add(node.Token, $"Unsupported operator '{node.Token.Lexeme}'.");
            throw new RuntimeException(Errors);
        }
    }

    public object? Visit(CallExpression node) => throw new NotImplementedException();

    public object? Visit(GetExpression node) => throw new NotImplementedException();

    public object? Visit(GroupExpressionNode node) => node.Inner.Accept(this);

    public object? Visit(IdentifierExpression node)
    {
        if (_env.TryFind(node.Token, out var value))
        {
            return value;
        }
        
        Errors.Add(node.Token, $"Unknown identifier {node.Token.Lexeme}");
        throw new RuntimeException(Errors);
        
    }

    public object? Visit(LiteralExpression node) => node.Token.Value;

    public object? Visit(LogicalExpression node)
    {
        var left = node.Left.Accept(this);

        return node.Token.Lexeme switch
        {
            "and" => !left.AsBool() ? left : Eval(node.Right),
            "or"  => left.AsBool() ? left : Eval(node.Right),
            _     => throw new UnreachableException()
        };

        object? Eval(ExpressionNode n) => n.Accept(this);
    }

    public object? Visit(SetExpression node) => throw new NotImplementedException();

    public object? Visit(SuperExpression node) => throw new NotImplementedException();

    public object? Visit(ThisExpression node) => throw new NotImplementedException();

    public object? Visit(UnaryExpression node) =>
        node.Token.Lexeme switch
        {
            "-" => this.Negate(node.Operand),
            "!" => this.Not(node.Operand),
            _   => throw new UnreachableException()
        };

    public object? Visit(BlockStatement node) => throw new NotImplementedException();

    public object? Visit(ExpressionStatement node) => throw new NotImplementedException();

    public object? Visit(IfStatement node) => throw new NotImplementedException();

    public object? Visit(PrintStatement node)
    {
        var output = node.Value.Accept(this);

        _textWriter.WriteLine(Stringify(output));
        return null;
    }

    public object? Visit(ReturnStatement node) => throw new NotImplementedException();

    public object? Visit(WhileStatement node) => throw new NotImplementedException();

    public object? Visit(ProgramNode node)
    {
        foreach (var stmt in node.Statements)
        {
            stmt.Accept(this);
        }

        return null;
    }

    #endregion
}