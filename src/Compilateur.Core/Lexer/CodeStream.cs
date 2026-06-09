namespace Compilateur.Core.Lexer;

public sealed class CodeStream
{
    #region Fields

    private int _currentColumn = 1;

    private int _currentIndex;
    private int _currentLine = 1;
    private readonly string _source;

    private static readonly char?[] NewLines = ['\n', '\r'];

    #endregion

    #region Constructors

    public CodeStream(string source) => _source = source;

    #endregion

    #region Properties

    private bool IsNextEof => _currentIndex + 1 >= _source.Length;
    public bool IsEof => _currentIndex >= _source.Length;

    #endregion

    #region Methods

    private (bool IsNewLine, int Offset) IsNextNewLine()
    {
        var current = Peek();
        var hasNext = TryPeekNext(out var next);

        if (hasNext)
        {
            if (current.Char == '\r' && next!.Char == '\n')
            {
                return (true, 2);
            }
        }

        return NewLines.Contains(current.Char)
            ? (true, 1)
            : (false, 1);
    }

    private bool TryPeek(out CodeChar? value)
    {
        if (_currentIndex < 0 || _currentIndex >= _source.Length)
        {
            value = null;
            return false;
        }

        value = new CodeChar
        {
            Char = _source[_currentIndex],
            Column = _currentColumn,
            Line = _currentLine
        };
        return true;
    }

    private bool TryPeekNext(out CodeChar? value)
    {
        if (IsNextEof)
        {
            value = null;
            return false;
        }

        value = new CodeChar
        {
            Char = _source[_currentIndex + 1],
            Line = _currentLine,
            Column = _currentColumn
        };
        return true;
    }

    public CodeChar Consume()
    {
        if (IsEof) return CodeChar.Empty;

        var readValue = Peek();

        var isNewLine = IsNextNewLine();
        if (isNewLine.IsNewLine)
        {
            _currentLine++;
            _currentColumn = 1;
        }
        else
        {
            _currentColumn++;
        }

        _currentIndex += isNewLine.Offset;
        return readValue;
    }

    public CodeChar Peek()
    {
        var read = TryPeek(out var value)
            ? value
            : throw new IndexOutOfRangeException(
                $"Cannot peek at index {_currentIndex}: source length is {_source.Length}.");
        return read!;
    }

    public CodeChar? PeekNext() =>
        TryPeekNext(out var value)
            ? value
            : CodeChar.Empty;

    #endregion
}