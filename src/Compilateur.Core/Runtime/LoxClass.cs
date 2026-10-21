using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public class LoxClass
{
    #region Constructors

    public LoxClass(ClassDeclarationStatement declaration, Frame closure)
    {
        Declaration = declaration;
        Closure = closure;
    }

    #endregion

    #region Properties

    public Frame Closure { get; }

    public ClassDeclarationStatement Declaration { get; }

    #endregion
}