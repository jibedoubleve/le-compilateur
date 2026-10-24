using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public class LoxFunction
{
    #region Constructors

    public LoxFunction(FunctionDeclarationStatement declaration, Frame closure, bool isInitialiser = false)
    {
        Declaration = declaration;
        Closure = closure;
        IsInitialiser = isInitialiser;
    }

    #endregion

    #region Properties

    public int Arity => Declaration.Parameters.Count;
    public Frame Closure { get; }
    public FunctionDeclarationStatement Declaration { get; }
    public bool IsInitialiser { get; }

    #endregion
}