using System.Collections.ObjectModel;
using Compilateur.Core.Errors;
using Compilateur.Core.Semantic.Helpers;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Semantic;

public class Resolver : ISyntaxNodeVisitor<Unit>
{
    #region Fields

    private const string SuperLexeme = "super";

    private const string ThisLexeme = "this";

    private readonly Dictionary<SyntaxNode, uint> _distanceMap = new(ReferenceEqualityComparer.Instance);

    private readonly Enclosing _enclosing = new();
    private readonly ScopeManager _scopes = new();

    #endregion

    #region Constructors

    public Resolver(ErrorCollection errors) => Errors = errors;

    #endregion

    #region Properties

    public ReadOnlyDictionary<SyntaxNode, uint> DistanceMap => _distanceMap.AsReadOnly();

    public ErrorCollection Errors { get; }

    #endregion

    #region Methods

    private void VisitFunctionBody(FunctionDeclarationStatement node, bool isMethod = false)
    {
        var enter = (node.Token.Lexeme, isMethod) switch
        {
            ("init", true) => _enclosing.EnterInitialiser(),
            _              => _enclosing.EnterFunction()
        };
        using (enter)
        {
            _scopes.Push(scope => {
                // declare parameters
                foreach (var param in node.Parameters)
                {
                    if (scope.IsDeclared(param.Lexeme))
                    {
                        Errors.Add(param, "Already a variable with this name in this scope.");
                    }

                    scope.Declare(param.Lexeme);
                    scope.Define(param.Lexeme);
                }

                // declare body
                foreach (var stmt in node.Body.Statements)
                {
                    stmt.Accept(this);
                }
            });
        }
    }

    public Unit Visit(ClassDeclarationStatement node)
    {
        var enclosing = node.SuperClass is null
            ? _enclosing.EnterClass()
            : _enclosing.EnterSuperclass();

        // declare class
        _scopes.OnCurrent(scope => {
            var name = node.Token.Lexeme;
            scope.Declare(name);
            scope.Define(name);
        });

        using (enclosing)
        {
            // resolve super class
            node.SuperClass?.Accept(this);

            using (node.SuperClass is not null ? _scopes.Push() : null)
            {
                if (node.SuperClass is not null)
                {
                    _scopes.OnCurrent(s => {
                        if (node.SuperClass.Token.Lexeme == node.Token.Lexeme)
                        {
                            Errors.Add(node.Token, "A class can't inherit from itself.");
                        }

                        s.Declare(SuperLexeme);
                        s.Define(SuperLexeme);
                    });
                }

                // declare 'this'
                _scopes.Push(u => {
                    u.Declare(ThisLexeme);
                    u.Define(ThisLexeme);

                    // declare functions
                    if (node.Functions.Count == 0) { return; }

                    foreach (var function in node.Functions)
                    {
                        VisitFunctionBody(function, true);
                    }
                });
            }
        }

        return Unit.Value;
    }

    public Unit Visit(FunctionDeclarationStatement node)
    {
        _scopes.OnCurrent(scope => {
            scope.Declare(node.Token.Lexeme);
            scope.Define(node.Token.Lexeme);
        });
        VisitFunctionBody(node);
        return Unit.Value;
    }

    public Unit Visit(VarDeclarationStatement node)
    {
        var name = node.Token.Lexeme;
        _scopes.OnCurrent(scope => {
            if (scope.IsDeclared(name))
            {
                Errors.Add(node.Token, "Already a variable with this name in this scope.");
            }

            scope.Declare(name);
        });

        node.Initialiser?.Accept(this);

        _scopes.OnCurrent(scope => scope.Define(name));
        return Unit.Value;
    }

    public Unit Visit(AssignExpression node)
    {
        node.Target.Accept(this);
        node.Value.Accept(this);
        return Unit.Value;
    }

    public Unit Visit(BinaryExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);
        return Unit.Value;
    }

    public Unit Visit(CallExpression node)
    {
        node.Callee.Accept(this);

        if (node.Arguments.Count > Maximum.Args)
        {
            Errors.Add(
                node.Arguments.Last().Token,
                $"Can't have more than {Maximum.Args} arguments."
            );
        }

        foreach (var argument in node.Arguments)
        {
            argument.Accept(this);
        }

        return Unit.Value;
    }

    public Unit Visit(GetExpression node)
    {
        node.Object.Accept(this);
        return Unit.Value;
    }

    public Unit Visit(GroupExpressionNode node)
    {
        node.Inner.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(IdentifierExpression node)
    {
        _scopes.OnCurrent(scope => {
            if (scope.IsDeclared(node.Token.Lexeme) &&
                !scope.IsDefined(node.Token.Lexeme))
            {
                Errors.Add(node.Token, "Can't read local variable in its own initializer.");
            }
        });

        _scopes.ComputeDistance(
            node,
            (n, d) => _distanceMap.Add(n, d)
        );

        return Unit.Value;
    }

    public Unit Visit(LiteralExpression node) => Unit.Value; //Nothing to do for a literal

    public Unit Visit(LogicalExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(SetExpression node)
    {
        node.Object.Accept(this);
        node.Value.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(SuperExpression node)
    {
        switch (_enclosing.Class)
        {
            case ClassType.None:
                Errors.Add(node.Token, "Can't use 'super' outside of a class.");
                break;
            case ClassType.Class:
                Errors.Add(node.Token, "Can't use 'super' in a class with no superclass.");
                break;
        }

        _scopes.ComputeDistance(node, (n, d) => _distanceMap.Add(n, d));

        return Unit.Value;
    }

    public Unit Visit(ThisExpression node)
    {
        if (_enclosing.Class is not (ClassType.Class or ClassType.Subclass))
        {
            Errors.Add(node.Token, "Can't use 'this' outside of a class.");
        }

        _scopes.ComputeDistance(node, (n, d) => _distanceMap.Add(n, d));

        return Unit.Value;
    }

    public Unit Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(BlockStatement node)
    {
        _scopes.Push(_ => {
            foreach (var stmt in node.Statements)
            {
                stmt.Accept(this);
            }
        });

        return Unit.Value;
    }

    public Unit Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        return Unit.Value;
    }

    public Unit Visit(IfStatement node)
    {
        node.Condition.Accept(this);
        node.ThenBranch.Accept(this);
        node.ElseBranch?.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(PrintStatement node)
    {
        node.Value.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(ReturnStatement node)
    {
        switch (_enclosing.Function)
        {
            case FunctionType.Initialiser:

                if (node.Expression is not null)
                {
                    Errors.Add(node.Token, "Can't return a value from an initializer.");
                }

                break;
            case FunctionType.Function: break;
            default:
                Errors.Add(node.Token, "Can't return from top-level code.");
                break;
        }

        node.Expression?.Accept(this);

        return Unit.Value;
    }

    public Unit Visit(WhileStatement node)
    {
        node.Condition?.Accept(this);
        node.Body.Accept(this);
        return Unit.Value;
    }

    public Unit Visit(ProgramNode node)
    {
        foreach (var statement in node.Statements)
        {
            statement.Accept(this);
        }

        return Unit.Value;
    }

    #endregion

    private enum FunctionType { None, Function, Initialiser }

    private enum ClassType { None, Class, Subclass }

    private sealed class Enclosing
    {
        #region Properties

        public ClassType Class { get; private set; } = ClassType.None;

        public FunctionType Function { get; private set; } = FunctionType.None;

        #endregion

        #region Methods

        public IDisposable EnterClass() => new ScopeGuard<ClassType>(
            () => {
                var cache = Class;
                Class = ClassType.Class;
                return cache;
            },
            cache => Class = cache
        );

        public IDisposable EnterFunction() => new ScopeGuard<FunctionType>(
            () => {
                var cache = Function;
                Function = FunctionType.Function;
                return cache;
            },
            cache => Function = cache
        );

        public IDisposable EnterInitialiser() => new ScopeGuard<FunctionType>(
            () => {
                var cache = Function;
                Function = FunctionType.Initialiser;
                return cache;
            },
            cache => Function = cache
        );

        public IDisposable EnterSuperclass() => new ScopeGuard<ClassType>(
            () => {
                var cache = Class;
                Class = ClassType.Subclass;
                return cache;
            },
            cache => Class = cache
        );

        #endregion
    }
}