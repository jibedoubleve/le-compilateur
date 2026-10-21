using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public class LoxFunction
{
    #region Constructors

    public LoxFunction(FunctionDeclarationStatement declaration, Frame closure)
    {
        Declaration = declaration;
        Closure = closure;
    }

    #endregion

    #region Properties

    public int Arity => Declaration.Parameters.Count;
    public Frame Closure { get; }
    public FunctionDeclarationStatement Declaration { get; }

    #endregion
}