using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Runtime;

public class Environment
{
    #region Fields

    private Frame _current = new();

    #endregion

    #region Methods

    private void PopFrame() =>
        _current = _current.Parent
                   ?? throw new InvalidOperationException("Cannot exit the global frame.");

    private void PushFrame() => _current = new Frame(_current);

    private static bool TryGetValue(string lexeme, Frame frame, out object? result)
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

    public bool TryDefine(string name, object? value) => _current.Define(name, value);

    public bool TryGetValue(Token token, out object? result) => TryGetValue(token.Lexeme, _current, out result);

    public bool TryAssign(Token token, object? value)
    {
        var frame = _current;
        while (frame is not null)
        {
            var found = frame.Update(token.Lexeme, value);
            if (!found) { frame = frame.Parent; }
            else { return true; }
        }

        return false;
    }

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