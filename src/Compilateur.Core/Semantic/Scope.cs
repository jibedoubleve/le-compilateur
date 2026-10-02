namespace Compilateur.Core.Semantic;

public class Scope
{
    #region Fields

    private readonly Dictionary<string, bool> _variables = new();

    #endregion

    #region Methods

    public void Declare(string name) => _variables[name] = false;
    public void Define(string name) => _variables[name] = true;

    public bool IsDeclared(string name) => _variables.ContainsKey(name);
    public bool IsDefined(string name) => _variables.GetValueOrDefault(name, false);

    #endregion
}