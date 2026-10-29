using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime.Native;

public class ClockFunction : ILoxCallable
{
    #region Properties

    public int Arity => 0;

    #endregion

    #region Methods

    public object? Call(Runtime runtime, ExpressionNode node, object?[] args)
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

    public override string ToString() => "<native fn>";

    #endregion
}