namespace Compilateur.Core.Semantic;

public class Scope
{
    #region Fields

    private readonly Dictionary<string, bool> _symbols = new();

    #endregion

    #region Methods

    public void Declare(string name) => _symbols[name] = false;
    public void Define(string name) => _symbols[name] = true;

    public bool IsDeclared(string name) => _symbols.ContainsKey(name);
    public bool IsDefined(string name) => _symbols.GetValueOrDefault(name, false);

    #endregion
}