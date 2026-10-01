using System.Collections;
using Compilateur.Core.Lexical;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors;

public class CompilationErrorCollection : IEnumerable<CompilationError>
{
    #region Fields

    private readonly List<CompilationError> _errors = new();

    #endregion

    #region Properties

    public IReadOnlyCollection<CompilationError> Errors => _errors.ToArray();

    #endregion

    #region Methods

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(CompilationError error) => _errors.Add(error);

    public void Add(CodeChar? token, string message) => _errors.Add(token is not null
        ? new CompilationError(token, message)
        : new CompilationError(message));

    public void Add(Token? token, string message) => _errors.Add(token is not null
        ? new CompilationError(token, message)
        : new CompilationError(message));

    public IEnumerator<CompilationError> GetEnumerator() => _errors.GetEnumerator();

    #endregion
}