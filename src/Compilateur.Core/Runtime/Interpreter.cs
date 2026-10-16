using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime;

public class Interpreter
{
    #region Methods

    public ErrorCollection Interpret(ProgramNode program)
    {
        var evaluator = new Evaluator();
        program.Accept(evaluator);
        return evaluator.Errors;
    }

    #endregion
}