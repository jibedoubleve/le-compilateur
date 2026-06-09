using System.Collections;

namespace Compilateur.Core.Lexical;

public class SyntaxErrorCollection : IEnumerable<SyntaxError>
{
    #region Fields

    private readonly List<SyntaxError> _errors = new();

    #endregion

    #region Methods

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Add(SyntaxError error) => _errors.Add(error);
    public IEnumerator<SyntaxError> GetEnumerator() => _errors.GetEnumerator();

    #endregion
}