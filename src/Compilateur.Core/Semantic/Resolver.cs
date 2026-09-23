namespace Compilateur.Core.Semantic;

public class Scope
{
    #region Fields

    private readonly Dictionary<string, bool> _scopes = new();

    #endregion

    #region Methods

    public void Declare(string name) => _scopes[name] = false;
    public void Define(string name) => _scopes[name] = true;

    public bool IsDeclared(string name) => _scopes.ContainsKey(name);
    public bool IsDefined(string name) => _scopes.GetValueOrDefault(name, false);

    #endregion
}

public class Resolver
{
    #region Fields

    private readonly Stack<Scope> _scopes = new();

    #endregion
}