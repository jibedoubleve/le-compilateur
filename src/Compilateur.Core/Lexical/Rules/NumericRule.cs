using System.Globalization;
using System.Text;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Lexical;

namespace Compilateur.Core.Lexical.Rules;

public sealed record NumericRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 25;

    private static readonly char[] Separators = ['.', ','];

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    private static bool IsEndOfScan(CodeStream codeStream)
    {
        if (codeStream.IsEof)
        {
            return true;
        }

        var codeChar = codeStream.Peek();
        return !IsValidChar(codeChar);
    }

    private static bool IsValidChar(char? codeChar) => codeChar.HasValue &&
                                                       (char.IsAsciiDigit(codeChar.Value) ||
                                                        Separators.Contains(codeChar.Value));


    public bool Matches(CodeStream codeStream)
    {
        var current = codeStream.Peek();
        return !current.IsEmpty && char.IsAsciiDigit(current.Char!.Value);
    }

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var strBuilder = new StringBuilder();
        var first = codeStream.Peek();
        var decimalCounter = 0;

        while (!IsEndOfScan(codeStream))
        {
            var current = codeStream.Consume();

            if (current.Char.HasValue && Separators.Contains(current.Char.Value))
            {
                decimalCounter++;
            }

            strBuilder.Append(current.Char);
        }

        var lexeme = strBuilder.ToString();
        if (decimalCounter > 1)
        {
            errors?.Add(new SyntaxError
            {
                Column = first.Column,
                Line = first.Line,
                Message = $"Malformed number literal '{lexeme}': multiple decimal points."
            });
            return null;
        }

        return new Token
        {
            Column = first.Column,
            Line = first.Line,
            Lexeme = lexeme,
            Value = double.Parse(lexeme, CultureInfo.InvariantCulture),
            Type = TokenType.Numeric
        };
    }

    #endregion
}