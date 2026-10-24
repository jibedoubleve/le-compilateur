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

    public object? GetProperty(string name)
    {
        var field = _variables.GetValueOrDefault(name);
        if (field is not null) { return field; }

        var fun = Class.FindMethod(name);
        if (fun is null) { return null; }

        var frame = new Frame(fun.Closure);
        frame.Define("this", this);
        return new LoxFunction(fun.Declaration, frame, fun.IsInitialiser);
    }

    #endregion
}