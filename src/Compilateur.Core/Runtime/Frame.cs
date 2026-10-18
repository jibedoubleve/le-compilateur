namespace Compilateur.Core.Runtime;

public class Frame
{
    #region Fields

    private readonly Dictionary<string, object?> _bindings = new();

    #endregion

    #region Constructors

    public Frame(Frame? parent = null) => Parent = parent;

    #endregion

    #region Properties

    public Frame? Parent { get; }

    #endregion

    #region Methods

    public bool Contains(string name) => _bindings.ContainsKey(name);

    public void Define(string name, object? value = null)
    {
        // If I'm in the global frame and the var
        // already exists then update its value
        if (Parent == null && !_bindings.TryAdd(name, value))
        {
            _bindings[name] = value;
        }
    }

    public object? Get(string name) => _bindings[name];

    #endregion
}