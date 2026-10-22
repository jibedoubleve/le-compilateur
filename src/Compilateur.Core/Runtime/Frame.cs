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

    public bool Define(string name, object? value = null)
    {
        // If in global scope, then override if exists
        if (Parent is null)
        {
            _bindings[name] = value;
            return true;
        }

        return _bindings.TryAdd(name, value);
    }

    public object? Get(string name)
    {
        var bindings = _bindings;
        while (true)
        {
            if (bindings?.TryGetValue(name, out var value) ?? false)
            {
                return value;
            }

            bindings = Parent?._bindings;
        }
    }

    public bool Update(string name, object? value)
    {
        if (!_bindings.ContainsKey(name)) { return false; }

        _bindings[name] = value;
        return true;
    }

    #endregion
}