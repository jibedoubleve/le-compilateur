using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Semantic;

public class Resolver
{
    #region Fields

    private readonly ErrorCollection _errors;

    #endregion

    #region Constructors

    public Resolver(ErrorCollection errors) => _errors = errors;

    #endregion

    #region Methods

    public ResolveResult Resolve(ProgramNode program)
    {
        var binder = new Binder(_errors);
        program.Accept(binder);
        return new ResolveResult(binder.Errors, binder.DistanceMap);
    }

    #endregion
}