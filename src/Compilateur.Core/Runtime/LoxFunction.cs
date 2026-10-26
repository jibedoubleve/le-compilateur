using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Runtime;

public class LoxFunction : ILoxCallable
{
    #region Constructors

    public LoxFunction(FunctionDeclarationStatement declaration, Frame closure, bool isInitialiser = false)
    {
        Declaration = declaration;
        Closure = closure;
        IsInitialiser = isInitialiser;
    }

    #endregion

    #region Properties

    public int Arity => Declaration.Parameters.Count;
    public Frame Closure { get; }
    public FunctionDeclarationStatement Declaration { get; }
    public bool IsInitialiser { get; }

    #endregion

    #region Methods

    public object? Call(Runtime runtime, ExpressionNode node, object?[] args)
    {
        using (runtime.Environment.EnterFrame(Closure))
        {
            var parameters = Declaration.Parameters;

            if (Arity != args.Length)
            {
                throw runtime.Errors
                             .RuntimeException(node, $"Expected {Arity} arguments but got {args.Length}.");
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                runtime.Environment.TryDefine(
                    parameters[i].Lexeme,
                    args[i]
                );
            }

            var result = RunFunction();
            return IsInitialiser
                ? runtime.Environment.CurrentFrame.Get("this")
                : result;
        }

        object? RunFunction()
        {
            try
            {
                return Declaration.Body.Accept(runtime);
            }
            catch (ReturnException ex)
            {
                return ex.ReturnValue;
            }
        }
    }

    public override string ToString() => $"<fn {Declaration.Token.Lexeme}>";

    #endregion
}