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
        TryGet(name, out var result);
        return result;
    }

    public bool TryGet(string name, out object? val)
    {
        var frame = this;

        while (frame is not null)
        {
            if (frame?._bindings.TryGetValue(name, out var value) ?? false)
            {
                val = value;
                return true;
            }

            frame = frame?.Parent;
        }

        val = null;
        return false;
    }

    public bool TryUpdate(string name, object? value)
    {
        if (!_bindings.ContainsKey(name)) { return false; }

        _bindings[name] = value;
        return true;
    }

    #endregion
}