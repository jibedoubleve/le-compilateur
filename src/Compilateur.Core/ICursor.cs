namespace Compilateur.Core;

public interface ICursor<out T>
{
    #region Properties

    bool IsAtEnd { get; }

    #endregion

    #region Methods

    T Consume();
    T Peek();
    T? PeekNext();

    #endregion
}