using System.Collections.ObjectModel;
using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core;

public record Analysis
{
    #region Constructors

    private Analysis(ErrorCollection errors, ProgramNode? ast, ReadOnlyDictionary<SyntaxNode, uint>? distanceMap)
    {
        Ast = ast;
        Errors = errors;
        DistanceMap = distanceMap;
    }

    #endregion

    #region Properties

    public ProgramNode? Ast { get; }
    public ReadOnlyDictionary<SyntaxNode, uint>? DistanceMap { get; }

    public ErrorCollection Errors { get; }
    public bool HasErrors => Errors.Any();

    #endregion

    #region Methods

    public static Analysis Error(ErrorCollection errors) => new(errors, null, null);

    public static Analysis Success(ProgramNode node, ReadOnlyDictionary<SyntaxNode, uint> distanceMap) =>
        new([], node, distanceMap);

    #endregion
}