using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public class Resolver
{
    #region Methods

    public ResolveResult Resolve(ProgramNode program)
    {
        var binder = new Binder();
        program.Accept(binder);
        return new ResolveResult(binder.Errors, binder.DistanceMap);
    }

    #endregion
}