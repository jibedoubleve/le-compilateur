using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public record LoxFunction(FunctionDeclarationStatement Function, Frame Closure)
{
    public int Arity => Function.Parameters.Count;
}