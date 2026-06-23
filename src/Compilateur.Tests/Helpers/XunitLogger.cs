using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Compilateur.Tests.Helpers;

public static class XunitLoggerExtensions
{
    #region Methods

    public static ILogger ToLogger(this ITestOutputHelper source) => new XunitLogger(source);

    #endregion
}

public sealed class XunitLogger : ILogger
{
    #region Fields

    private readonly string _categoryName;
    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public XunitLogger(ITestOutputHelper output, string categoryName = "XunitLogger")
    {
        _output = output;
        _categoryName = categoryName;
    }

    #endregion

    #region Methods

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _output.WriteLine($"{logLevel}: {_categoryName}[{eventId.Id}] {formatter(state, exception)}");
        if (exception is not null)
        {
            _output.WriteLine(exception.ToString());
        }
    }

    #endregion

    private sealed class NullScope : IDisposable
    {
        #region Fields

        public static readonly NullScope Instance = new();

        #endregion

        #region Methods

        public void Dispose() { }

        #endregion
    }
}