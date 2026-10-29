namespace Compilateur.Core.Runtime;

public class Interpreter
{
    #region Fields

    private readonly Runtime _runtime;

    #endregion

    #region Constructors

    public Interpreter(TextWriter textWriter) => _runtime = new Runtime(textWriter);

    #endregion

    #region Methods

    public void Interpret(Analysis analysis)
    {
        if (analysis.HasErrors || analysis.Ast is null)
        {
            throw new ArgumentException("Invalid analysis", nameof(analysis));
        }

        _runtime.Reset(analysis);
        analysis.Ast.Accept(_runtime);
    }

    #endregion
}