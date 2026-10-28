using System.Diagnostics;

namespace Compilateur.Core.Runtime;

public class Environment
{
    #region Constructors

    public Environment() => CurrentFrame = GlobalFrame = new Frame();

    #endregion

    #region Properties

    private Frame GlobalFrame { get; }

    public Frame CurrentFrame { get; private set; }

    #endregion

    #region Methods

    private Frame GetFrame(uint jmp)
    {
        var frame = CurrentFrame;
        for (var i = 0; i < jmp; i++)
        {
            frame = frame?.Parent;
        }

        return frame ?? throw new UnreachableException();
    }

    private void PopFrame(Frame? previousFrame) =>
        CurrentFrame = previousFrame
                       ?? CurrentFrame.Parent
                       ?? throw new InvalidOperationException("Cannot exit the global frame.");

    private void PushFrame() => CurrentFrame = new Frame(CurrentFrame);

    private void PushFrame(Frame frame) => CurrentFrame = new Frame(frame);

    public IDisposable EnterFrame(Frame closure) => new FrameLifecycle(this, closure, CurrentFrame);

    public IDisposable EnterFrame() => new FrameLifecycle(this);

    public bool TryAssign(uint? jmp, string lexeme, object? value) =>
        jmp is null
            ? GlobalFrame.TryUpdate(lexeme, value)
            : GetFrame(jmp.Value).TryUpdate(lexeme, value);

    public bool TryDefine(string name, object? value) => CurrentFrame.Define(name, value);

    public bool TryGetValue(uint? jmp, string lexeme, out object? value) =>
        jmp is null
            ? GlobalFrame.TryGet(lexeme, out value)
            : GetFrame(jmp.Value).TryGet(lexeme, out value);

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