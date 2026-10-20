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

    private readonly Environment _env = new();

    private readonly TextWriter _textWriter;

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

    public object? Visit(FunctionDeclarationStatement node)
    {
        var fun = new LoxFunction(
            node,
            _env.CurrentFrame
        );
        _env.TryDefine(node.Token.Lexeme, fun);
        return null;
    }

    public object? Visit(VarDeclarationStatement node)
    {
        var value = node.Initialiser?.Accept(this);
        var name = node.Token.Lexeme;

        var isSuccess = _env.TryDefine(name, value);

        return isSuccess
            ? null
            : throw new UnreachableException();
    }

    public object? Visit(AssignExpression node)
    {
        var target = node.Target.Token;
        var value = node.Value.Accept(this);

        var success = _env.TryAssign(target, value);

        if (!success)
        {
            Errors.Add(node.Token, $"Undefined variable '{target.Lexeme}'.");
            throw new RuntimeException(Errors);
        }

        return value;
    }

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
            ("+", double x, double y)  => x + y,
            ("+", string x, string y)  => x + y,
            ("-", double x, double y)  => x - y,
            ("*", double x, double y)  => x * y,
            ("/", double x, double y)  => x / y,
            ("<", double x, double y)  => x < y,
            ("<=", double x, double y) => x <= y,
            (">", double x, double y)  => x > y,
            (">=", double x, double y) => x >= y,
            _                          => Fail()
        };

        object Fail()
        {
            Errors.Add(node.Token, $"Unsupported operator '{node.Token.Lexeme}'.");
            throw new RuntimeException(Errors);
        }
    }

    public object? Visit(CallExpression node)
    {
        if (node.Callee.Accept(this) is not LoxFunction fun)
        {
            throw new InvalidOperationException();
        }

        var arguments = node.Arguments
                            .Select( a => a.Accept(this) )
                            .ToList();
        
        using (_env.EnterFrame(fun.Closure))
        {
            var parameters = fun.Function.Parameters;

            if (fun.Arity != arguments.Count)
            {
                Errors.Add(
                    node.Token,
                    $"Expected {fun.Arity} arguments but got {arguments.Count}."
                );
                throw new RuntimeException(Errors);
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                _env.TryDefine(
                    parameters[i].Lexeme,
                    arguments[i]
                );
            }

            return fun.Function.Body.Accept(this);
        }
    }

    public object? Visit(GetExpression node) => throw new NotImplementedException();

    public object? Visit(GroupExpressionNode node) => node.Inner.Accept(this);

    public object? Visit(IdentifierExpression node)
    {
        if (_env.TryGetValue(node.Token, out var value))
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

    public object? Visit(BlockStatement node)
    {
        using (_env.EnterFrame())
        {
            foreach (var stmt in node.Statements)
            {
                stmt.Accept(this);
            }
        }

        return null;
    }

    public object? Visit(ExpressionStatement node) => node.Expression.Accept(this);

    public object? Visit(IfStatement node)
    {
        var condition = node.Condition.Accept(this);
        return condition.AsBool()
            ? node.ThenBranch.Accept(this)
            : node.ElseBranch?.Accept(this);
    }

    public object? Visit(PrintStatement node)
    {
        var output = node.Value.Accept(this);

        _textWriter.WriteLine(Stringify(output));
        return null;
    }

    public object? Visit(ReturnStatement node) => throw new NotImplementedException();

    public object? Visit(WhileStatement node)
    {
        while (node.Condition?.Accept(this).AsBool() ?? true)
        {
            node.Body.Accept(this);
        }

        return null;
    }

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