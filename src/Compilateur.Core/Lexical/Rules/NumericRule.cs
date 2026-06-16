using System.Globalization;
using System.Text;
using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Errors.Rules;

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

    private static bool IsEndOfScan(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return true;
        }

        var codeChar = codeCursor.Peek();
        return !IsValidChar(codeChar);
    }

    private static bool IsValidChar(char? codeChar) => codeChar.HasValue &&
                                                       (char.IsAsciiDigit(codeChar.Value) ||
                                                        Separators.Contains(codeChar.Value));


    public bool Matches(CodeCursor codeCursor)
    {
        var current = codeCursor.Peek();
        return !current.IsEmpty && char.IsAsciiDigit(current.Char!.Value);
    }

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var strBuilder = new StringBuilder();
        var first = codeCursor.Peek();
        var decimalCounter = 0;

        while (!IsEndOfScan(codeCursor))
        {
            var current = codeCursor.Consume();

            if (current.Char.HasValue && Separators.Contains(current.Char.Value))
            {
                decimalCounter++;
            }

            strBuilder.Append(current.Char);
        }

        var lexeme = strBuilder.ToString();
        if (decimalCounter > 1)
        {
            errors?.Add(new SyntaxError(first, $"Malformed number literal '{lexeme}': multiple decimal points."));
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