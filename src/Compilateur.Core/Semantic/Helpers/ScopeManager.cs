using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic.Helpers;

public class ScopeManager
{
    #region Fields

    private readonly Stack<Scope> _scopes = new();

    #endregion

    #region Methods

    /// <summary>
    ///     Computes the distance between the node and the innermost scope that defines its name.
    ///     If such a scope exists, calls <paramref name="setter" /> with that distance (typically to
    ///     store it in the distance map). Otherwise, the name is assumed global and nothing is done.
    /// </summary>
    /// <param name="node">Node referencing a name.</param>
    /// <param name="setter">Callback receiving the node and its distance.</param>
    public void ComputeDistance(SyntaxNode node, Action<SyntaxNode, uint> setter)
    {
        var depth = 0U;
        var found = false;
        foreach (var scope in _scopes)
        {
            if (scope.IsDefined(node.Token.Lexeme))
            {
                found = true;
                break;
            }

            depth++;
        }

        if (found)
        {
            setter(node, depth);
        }
    }

    /// <summary>
    ///     Executes the action on the innermost scope, if any. At global level (empty stack),
    ///     the action is not executed at all.
    /// </summary>
    /// <param name="setter">Action applied to the innermost scope.</param>
    public void OnCurrent(Action<Scope> setter)
    {
        var current = _scopes.FirstOrDefault();
        if (current is not null)
        {
            setter(current);
        }
    }

    /// <summary>
    ///     Pushes a new empty scope, executes the action, then pops the scope.
    /// </summary>
    /// <param name="setter">Action executed while the new scope is the innermost one.</param>
    public void Push(Action<Scope> setter)
    {
        var scope = new Scope();
        _scopes.Push(scope);
        setter.Invoke(scope);
        _scopes.Pop();
    }

    public IDisposable Push() => new ScopeGuard(_scopes);

    #endregion

    private sealed class ScopeGuard : IDisposable
    {
        #region Fields

        private readonly Stack<Scope> _scopes;

        #endregion

        #region Constructors

        public ScopeGuard(Stack<Scope> scopes)
        {
            _scopes = scopes;
            _scopes.Push(new Scope());
        }

        #endregion

        #region Methods

        public void Dispose() => _scopes.Pop();

        #endregion
    }
}