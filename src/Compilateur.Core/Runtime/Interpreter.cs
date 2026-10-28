using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime;

public class Interpreter
{
    #region Fields

    private readonly TextWriter _textWriter;

    #endregion

    #region Constructors

    public Interpreter() : this(Console.Out) { }

    public Interpreter(TextWriter textWriter) => _textWriter = textWriter;

    #endregion

    #region Methods

    public ErrorCollection Interpret(Analysis analysis)
    {
        if (analysis.HasErrors || analysis.Ast is null)
        {
            throw new ArgumentException("Invalid analysis", nameof(analysis));
        }
        
        var evaluator = new Runtime(analysis, _textWriter);
        analysis.Ast.Accept(evaluator);
        return evaluator.Errors;
    }

    #endregion
}