using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core;

public record Analysis
{
    #region Constructors

    private Analysis(ProgramNode? ast, ErrorCollection errors)
    {
        Ast = ast;
        Errors = errors;
    }

    #endregion

    #region Properties

    public ErrorCollection Errors { get; }
    public bool HasErrors => Errors.Any();
    public ProgramNode? Ast { get; }

    #endregion

    #region Methods

    public static Analysis Error(ErrorCollection errors) => new(null, errors);

    public static Analysis Success(ProgramNode node) => new(node, []);

    #endregion
}