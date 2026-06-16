using System.Collections;

namespace Compilateur.Core.Errors;

public class SyntaxErrorCollection : IEnumerable<SyntaxError>
{
    #region Fields

    private readonly List<SyntaxError> _errors = new();

    #endregion

    #region Properties

    public IReadOnlyCollection<SyntaxError> Errors => _errors.ToArray();

    #endregion

    #region Methods

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Add(SyntaxError error) => _errors.Add(error);
    public IEnumerator<SyntaxError> GetEnumerator() => _errors.GetEnumerator();

    #endregion
}