using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime;

public interface ILoxCallable
{
    #region Properties

    int Arity { get; }

    #endregion

    #region Methods

    object? Call(Runtime runtime, ExpressionNode node, object?[] args);

    #endregion
}