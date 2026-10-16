using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime;

public class Interpreter
{
    #region Fields

    private readonly TextWriter _textWriter;

    #endregion

    #region Constructors

    public Interpreter(ErrorCollection errors)
    {
        _textWriter = Console.Out;
        Errors = errors;
    }

    public Interpreter(ErrorCollection errors, TextWriter textWriter)
    {
        _textWriter = textWriter;
        Errors = errors;
    }

    #endregion

    #region Properties

    public ErrorCollection Errors { get; }

    #endregion

    #region Methods

    public ErrorCollection Interpret(ProgramNode program)
    {
        var evaluator = new Evaluator(Errors, _textWriter);
        program.Accept(evaluator);
        return evaluator.Errors;
    }

    #endregion
}