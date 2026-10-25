namespace Compilateur.Core.Runtime;

public class LoxInstance
{
    #region Fields

    private readonly Dictionary<string, object?> _variables = new();

    #endregion

    #region Constructors

    public LoxInstance(LoxClass @class) => Class = @class;

    #endregion

    #region Properties

    public LoxClass Class { get; }

    #endregion

    #region Methods

    public void AddField(string name, object? value) => _variables[name] = value;

    public bool TryGetProperty(string name, out object? value)
    {
        var hasProperty = _variables.ContainsKey(name);
        
        var field = hasProperty ? _variables[name]  : null;

        if (hasProperty)
        {
            value = field;
            return true;
        }
        
        var fun = Class.FindMethod(name);
        if (fun is not null)
        {
            var frame = new Frame(fun.Closure);
            frame.Define("this", this);
            value = new LoxFunction(fun.Declaration, frame, fun.IsInitialiser);
            return true;
        }

        value = null;
        return false;
    }

    #endregion
}