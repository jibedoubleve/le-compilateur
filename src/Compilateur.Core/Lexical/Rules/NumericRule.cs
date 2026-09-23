using System.Globalization;
using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public sealed record NumericRule : ITokenRule
{
    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    public bool Matches(CodeCursor codeCursor)
    {
        var current = codeCursor.Peek();
        return !current.IsEmpty && char.IsAsciiDigit(current.Char!.Value);
    }

    public Token? Scan(CodeCursor cursor, SyntaxErrorCollection? errors = null)
    {
        var strBuilder = new StringBuilder();

        // A numeric should start with a number
        var first = cursor.Peek();
        if (!first.Char.HasValue || !char.IsAsciiDigit(first.Char.Value))
        {
            errors?.Add(first, $"Expected number literal, found '{first.Char ?? '\0'}'.");
            return null;
        }

        var hasSeparator = false;
        var current = first;
        while (!cursor.IsAtEnd)
        {
            // Then, the token should either be a numeric
            if (current.Char.HasValue && char.IsAsciiDigit(current.Char.Value))
            {
                strBuilder.Append(current.Char.Value);
                cursor.Consume();
                current = cursor.Peek();
                continue;
            }

            // or a '.' (dot) followed by a numeric
            var next = cursor.PeekNext();

            if (next is null) { break; }

            if (current.Char == '.' && next.Char.HasValue && char.IsAsciiDigit(next.Char.Value))
            {
                // If another separator, return the scanned numeric and go for the next...
                if (hasSeparator) { break; }

                hasSeparator = true;
                strBuilder.Append(current.Char);
                cursor.Consume();
                current = cursor.Peek();
                continue;
            }

            break;
        }

        var lexeme = strBuilder.ToString();
        return new Token
        {
            Column = first.Column,
            Line = first.Line,
            Lexeme = lexeme,
            Value = double.Parse(lexeme, CultureInfo.InvariantCulture),
            Kind = TokenKind.Numeric
        };
    }

    #endregion
}