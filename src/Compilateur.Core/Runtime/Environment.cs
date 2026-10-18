using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Runtime;

public class Environment
{
    #region Properties

    public Frame Current { get; } = new();
    public Frame Global => Current;

    #endregion

    #region Methods

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


    public bool TryFind(Token token, out object? result) => TryFind(token.Lexeme, Current, out result);

    #endregion
}