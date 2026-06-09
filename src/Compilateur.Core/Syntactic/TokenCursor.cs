using Compilateur.Core.Lexical;

namespace Compilateur.Core.Syntactic;

public class TokenCursor : ICursor<object>
{
    public bool IsAtEnd { get; }
    public object Consume() => throw new NotImplementedException();

    public object Peek() => throw new NotImplementedException();

    public object? PeekNext() => throw new NotImplementedException();
}