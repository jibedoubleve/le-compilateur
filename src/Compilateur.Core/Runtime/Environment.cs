using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Runtime;

public class Environment
{
    #region Properties

    private Frame Global => Current;

    public Frame Current { get; private set; } = new();

    #endregion

    #region Methods

    private void PopFrame() =>
        Current = Current.Parent
                  ?? throw new InvalidOperationException("Cannot exit the global frame.");

    private void PushFrame() => Current = new Frame(Current);

    private static bool TryFind(string lexeme, Frame frame, out object? result)
    {
        while (true)
        {
            if (frame.Contains(lexeme))
            {
                result = frame.Get(lexeme);
                return true;
            }

            if (frame.Parent is null)
            {
                result = null;
                return false;
            }

            frame = frame.Parent;
        }
    }

    public IDisposable EnterFrame() => new FrameLifecycle(this);

    public bool TryFind(Token token, out object? result) => TryFind(token.Lexeme, Current, out result);

    #endregion

    private sealed class FrameLifecycle : IDisposable
    {
        #region Fields

        private readonly Environment _env;

        #endregion

        #region Constructors

        public FrameLifecycle(Environment env)
        {
            _env = env;
            _env.PushFrame();
        }

        #endregion

        #region Methods

        public void Dispose() => _env.PopFrame();

        #endregion
    }
}