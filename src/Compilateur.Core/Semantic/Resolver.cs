using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public class Resolver
{
    #region Methods

    public void Resolve(ProgramNode program)
    {
        var binder = new Binder();
        program.Accept(binder);
    }

    #endregion
}