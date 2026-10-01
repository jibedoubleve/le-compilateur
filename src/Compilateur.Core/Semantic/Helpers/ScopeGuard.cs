namespace Compilateur.Core.Semantic.Helpers;

public sealed class ScopeGuard<T> : IDisposable
{
    #region Fields

    private readonly T? _cache;

    private readonly Action<T>? _onDispose;

    #endregion

    #region Constructors

    public ScopeGuard(Func<T> onInit, Action<T> onDispose)
    {
        _cache = onInit();
        _onDispose = onDispose;
    }

    #endregion

    #region Methods

    public void Dispose()
    {
        if (_cache is not null)
        {
            _onDispose?.Invoke(_cache);
        }
    }

    #endregion
}