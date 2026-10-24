using System.Diagnostics;
using Compilateur.Core.Errors;
using Compilateur.Core.Extensions;
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

    private object? VisitLoxCtor(LoxClass @class, CallExpression node)
    {
        var instance = new LoxInstance(@class);
        var ctor = instance.GetProperty("init");

        if (ctor is LoxFunction fun) { return VisitLoxFunction(fun, node); }

        return node.Arguments.Count == 0
            ? instance
            : throw Errors.RuntimeException(node.Token, $"Expected 0 arguments but got {node.Arguments.Count}.");
    }

    private object? VisitLoxFunction(LoxFunction fun, CallExpression node)
    {
        // Evaluate the arguments out of the closure.
        var arguments = node.Arguments
                            .Select(a => a.Accept(this))
                            .ToList();

        using (_env.EnterFrame(fun.Closure))
        {
            var parameters = fun.Declaration.Parameters;

            if (fun.Arity != arguments.Count)
            {
                throw Errors.RuntimeException(
                    node.Token,
                    $"Expected {fun.Arity} arguments but got {arguments.Count}."
                );
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                _env.TryDefine(
                    parameters[i].Lexeme,
                    arguments[i]
                );
            }

            var result = RunFunction();
            return fun.IsInitialiser
                ? _env.CurrentFrame.Get("this")
                : result;
        }

        object? RunFunction()
        {
            try
            {
                return fun.Declaration.Body.Accept(this);
            }
            catch (ReturnException ex)
            {
                return ex.ReturnValue;
            }
        }
    }

    public object? Visit(ClassDeclarationStatement node)
    {
        var @class = new LoxClass(
            node,
            _env.CurrentFrame,
            Errors
        );
        _env.TryDefine(node.Token.Lexeme, @class);
        return null;
    }

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
            Errors.RuntimeException(node.Token, $"Undefined variable '{target.Lexeme}'.");
        }

        return value;
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

            throw Errors.RuntimeException(node.Token, msg);
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
            ("==", var x, var y)       => Equals(x, y),
            ("!=", var x, var y)       => !Equals(x, y),
            _                          => Fail()
        };

        object Fail() => throw Errors.RuntimeException(node.Token, $"Unsupported operator '{node.Token.Lexeme}'.");

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

            return callee switch
            {
                LoxFunction fun => VisitLoxFunction(fun, node),
                LoxClass @class => VisitLoxCtor(@class, node),
                _               => throw new UnreachableException()
            };
        }
        catch (ReturnException ex) { return ex.ReturnValue; }
    }

    public object? Visit(GetExpression node)
    {
        var obj = node.Object.Accept(this);
        return obj switch
        {
            LoxInstance i => i.GetProperty(node.Token.Lexeme),
            _             => throw new UnreachableException()
        };
    }

    public object? Visit(GroupExpressionNode node) => node.Inner.Accept(this);

    public object? Visit(IdentifierExpression node)
    {
        if (_env.TryGetValue(node.Token, out var value))
        {
            return value;
        }

        throw Errors.RuntimeException(node.Token, $"Unknown identifier {node.Token.Lexeme}");
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
            throw new UnreachableException();
        }

        instance.AddField(
            node.Token.Lexeme,
            node.Value.Accept(this)
        );

        return null;
    }

    public object Visit(SuperExpression node)
    {
        _env.TryGetValue("super", out var super);
        if (super is not LoxClass superClass) { throw new UnreachableException(); }

        var method = superClass.FindMethod(node.Method.Lexeme);
        if (method is null)
        {
            throw Errors.RuntimeException(
                node.Token,
                $"Method '{node.Method.Lexeme}' not found."
            );
        }

        return method;
    }

    public object Visit(ThisExpression node)
    {
        _env.TryGetValue("this", out var value);
        return value ?? throw new UnreachableException();
    }

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