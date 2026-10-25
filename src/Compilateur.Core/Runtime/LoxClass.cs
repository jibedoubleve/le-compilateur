using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public class LoxClass
{
    #region Fields

    private readonly LoxClass? _super;

    #endregion

    #region Constructors

    public LoxClass(ClassDeclarationStatement declaration, Frame closure, ErrorCollection errors)
    {
        Declaration = declaration;
        Closure = closure;

        if (declaration.SuperClass is not { } super) { return; }

        if (closure.Get(super.Token.Lexeme) is not LoxClass loxClassSuper)
        {
            throw errors.RuntimeException(super, "Superclass must be a class.");
        }

        Closure = new Frame(closure);
        Closure.Define("super", loxClassSuper);
        _super = loxClassSuper;
    }

    #endregion

    #region Properties

    public Frame Closure { get; }

    public ClassDeclarationStatement Declaration { get; }

    #endregion

    #region Methods

    private static LoxFunction? FindMethod(LoxClass @this, string name, bool isCtor)
    {
        var super = @this._super;
        while (super is not null)
        {
            var method = super.FindMethod(name)?.Declaration;
            if (method is not null)
            {
                return new LoxFunction(method, super.Closure, isCtor);
            }

            super = super._super;
        }

        return null;
    }

    public LoxFunction? FindMethod(string name)
    {
        var isCtor = name == "init";
        var method = Declaration.Functions.SingleOrDefault(f => f.Token.Lexeme == name);

        return method is not null
            ? new LoxFunction(method, Closure, isCtor)
            : FindMethod(this, name, isCtor);
    }

    #endregion
}