using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Runtime;

public class Environment
{
    #region Properties

    public Frame CurrentFrame { get; private set; } = new();

    #endregion

    #region Methods

    private void PopFrame(Frame? previousFrame) =>
        CurrentFrame = previousFrame
                       ?? CurrentFrame.Parent
                       ?? throw new InvalidOperationException("Cannot exit the global frame.");

    private void PushFrame() => CurrentFrame = new Frame(CurrentFrame);

    private void PushFrame(Frame frame) => CurrentFrame = new Frame(frame);

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

    public IDisposable EnterFrame(Frame closure) => new FrameLifecycle(this, closure, CurrentFrame);

    public IDisposable EnterFrame() => new FrameLifecycle(this);

    public bool TryAssign(Token token, object? value)
    {
        var frame = CurrentFrame;
        while (frame is not null)
        {
            var found = frame.Update(token.Lexeme, value);
            if (!found) { frame = frame.Parent; }
            else { return true; }
        }

        return false;
    }

    public bool TryDefine(string name, object? value) => CurrentFrame.Define(name, value);

    public bool TryGetValue(Token token, out object? result) => TryGetValue(token.Lexeme, CurrentFrame, out result);

    #endregion

    private sealed class FrameLifecycle : IDisposable
    {
        #region Fields

        private readonly Environment _env;
        private readonly Frame? _previousFrame;

        #endregion

        #region Constructors

        public FrameLifecycle(Environment env, Frame closure, Frame previousFrame)
        {
            _env = env;
            _previousFrame = previousFrame;
            _env.PushFrame(closure);
        }

        public FrameLifecycle(Environment env)
        {
            _env = env;
            _env.PushFrame();
        }

        #endregion

        #region Methods

        public void Dispose() => _env.PopFrame(_previousFrame);

        #endregion
    }
}