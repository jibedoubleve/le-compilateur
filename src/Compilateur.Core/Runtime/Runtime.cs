using System.Diagnostics;
using Compilateur.Core.Errors;
using Compilateur.Core.Extensions;
using Compilateur.Core.Runtime.Extensions;
using Compilateur.Core.Runtime.Native;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Runtime;

public class Runtime : ISyntaxNodeVisitor<object?>
{
    #region Fields

    private readonly TextWriter _textWriter;

    #endregion

    #region Constructors

    public Runtime(ErrorCollection errors) : this(errors, Console.Out) { }

    public Runtime(ErrorCollection errors, TextWriter textWriter)
    {
        Errors = errors;
        _textWriter = textWriter;
        Environment.CurrentFrame.Define("clock", new ClockFunction());
    }

    #endregion

    #region Properties

    public Environment Environment { get; } = new();

    public ErrorCollection Errors { get; }

    #endregion

    #region Methods

    public object? Visit(ClassDeclarationStatement node)
    {
        var @class = new LoxClass(
            node,
            Environment.CurrentFrame,
            Errors
        );
        Environment.TryDefine(node.Token.Lexeme, @class);
        return null;
    }

    public object? Visit(FunctionDeclarationStatement node)
    {
        var fun = new LoxFunction(
            node,
            Environment.CurrentFrame
        );
        Environment.TryDefine(node.Token.Lexeme, fun);
        return null;
    }

    public object? Visit(VarDeclarationStatement node)
    {
        var value = node.Initialiser?.Accept(this);
        var name = node.Token.Lexeme;

        var isSuccess = Environment.TryDefine(name, value);

        return isSuccess
            ? null
            : throw new UnreachableException();
    }

    public object? Visit(AssignExpression node)
    {
        var target = node.Target.Token;
        var value = node.Value.Accept(this);

        var success = Environment.TryAssign(target, value);

        return success
            ? value
            : throw Errors.RuntimeException(node, $"Undefined variable '{target.Lexeme}'.");
    }

    public object? Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        if (!IsLogicalOp() && left?.GetType() != right?.GetType())
        {
            var msg = node.Token.Lexeme == "+"
                ? "Operands must be two numbers or two strings."
                : "Operands must be numbers.";

            throw Errors.RuntimeException(node, msg);
        }

        return (node.Token.Lexeme, left, right) switch
        {
            ("+", double x, double y)      => x + y,
            ("+", string x, string y)      => x + y,
            ("-", double x, double y)      => x - y,
            ("*", double x, double y)      => x * y,
            ("/", double x, double y)      => x / y,
            ("<", double x, double y)      => x < y,
            ("<=", double x, double y)     => x <= y,
            (">", double x, double y)      => x > y,
            (">=", double x, double y)     => x >= y,
            ("==", double.NaN, double.NaN) => false,
            ("!=", double.NaN, double.NaN) => true,
            ("==", var x, var y)           => Equals(x, y),
            ("!=", var x, var y)           => !Equals(x, y),
            _                              => Fail()
        };

        object Fail() => throw Errors.RuntimeException(node, "Operands must be two numbers or two strings.");

        bool IsLogicalOp()
        {
            IEnumerable<string> logicalOps = ["==", "!="];
            return logicalOps.Contains(node.Token.Lexeme);
        }
    }

    public object? Visit(CallExpression node)
    {
        try
        {
            var callee = node.Callee.Accept(this);

            if (callee is not ILoxCallable callable)
            {
                throw Errors.RuntimeException(node, "Can only call functions and classes.");
            }

            var arguments = node.Arguments
                                .Select(a => a.Accept(this))
                                .ToArray();

            return callable.Call(this, node, arguments);
        }
        catch (ReturnException ex) { return ex.ReturnValue; }
    }

    public object? Visit(GetExpression node)
    {
        var obj = node.Object.Accept(this);
        return obj switch
        {
            LoxInstance i => GetProperty(i),
            _             => throw Errors.RuntimeException(node, "Only instances have properties.")
        };

        object? GetProperty(LoxInstance instance) =>
            instance.TryGetProperty(node.Token.Lexeme, out var result)
                ? result
                : throw Errors.RuntimeException(node, $"Undefined property '{node.Token.Lexeme}'.");
    }

    public object? Visit(GroupExpressionNode node) => node.Inner.Accept(this);

    public object? Visit(IdentifierExpression node)
    {
        if (Environment.TryGetValue(node.Token, out var value))
        {
            return value;
        }

        throw Errors.RuntimeException(node, $"Undefined variable '{node.Token.Lexeme}'.");
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

    public object? Visit(SetExpression node)
    {
        var obj = node.Object.Accept(this);

        if (obj is not LoxInstance instance)
        {
            throw Errors.RuntimeException(node, "Only instances have fields.");
        }

        var value = node.Value.Accept(this);
        instance.AddField(
            node.Token.Lexeme,
            value
        );

        return value;
    }

    public object Visit(SuperExpression node)
    {
        Environment.TryGetValue("this", out var @this);
        if (@this is not LoxInstance instance) { throw new UnreachableException(); }

        Environment.TryGetValue("super", out var super);

        if (super is not LoxClass superClass) { throw new UnreachableException(); }

        var function = superClass.FindMethod(node.Method.Lexeme)
                       ?? throw Errors.RuntimeException(node, $"Undefined property '{node.Method.Lexeme}'.");

        return instance.Bind(function)
               ?? throw new UnreachableException();
    }

    public object? Visit(ThisExpression node) =>
        Environment.TryGetValue("this", out var value)
            ? value
            : throw new UnreachableException();

    public object? Visit(UnaryExpression node) =>
        node.Token.Lexeme switch
        {
            "-" => this.Negate(node.Operand),
            "!" => this.Not(node.Operand),
            _   => throw new UnreachableException()
        };

    public object? Visit(BlockStatement node)
    {
        using (Environment.EnterFrame())
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

        _textWriter.WriteLine(output.Stringify());
        return null;
    }

    public object Visit(ReturnStatement node) =>
        /* Stops execution of statements up to the call
         * This is caught in Visit(CallExpression)
         */
        throw new ReturnException(
            node.Expression?.Accept(this)
        );

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