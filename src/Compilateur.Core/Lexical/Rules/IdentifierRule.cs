using System.Text;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Lexical.Rules;

public sealed record IdentifierRule : ITokenRule
{
    #region Fields

    private const int MaxSize = 10_000;

    private readonly Dictionary<string, TokenKind> _keywords = new()
    {
        { "and", TokenKind.And },
        { "or", TokenKind.Or },
        { "nil", TokenKind.Nil },
        { "if", TokenKind.If },
        { "else", TokenKind.Else },
        { "while", TokenKind.While },
        { "for", TokenKind.For },
        { "fun", TokenKind.Fun },
        { "var", TokenKind.Var },
        { "class", TokenKind.Class },
        { "this", TokenKind.This },
        { "super", TokenKind.Super },
        { "return", TokenKind.Return },
        { "true", TokenKind.True },
        { "false", TokenKind.False },
        { "print", TokenKind.Print }
    };

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    private static bool IsValidChar(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return false;
        }

        char? character = codeCursor.Peek();
        return character.HasValue &&
               (char.IsAsciiLetterOrDigit(character.Value) || character == '_');
    }

    public bool Matches(CodeCursor codeCursor)
    {
        if (codeCursor.IsAtEnd)
        {
            return false;
        }

        char? character = codeCursor.Peek();
        return character.HasValue &&
               (char.IsAsciiLetter(character.Value) || character == '_');
    }

    public Token? Scan(CodeCursor codeCursor, SyntaxErrorCollection? errors = null)
    {
        var first = codeCursor.Consume();

        var strBuilder = new StringBuilder();
        strBuilder.Append(first.Char);

        for (var i = 0; i < MaxSize; i++)
        {
            if (!IsValidChar(codeCursor))
            {
                var lexeme = strBuilder.ToString();
                if (_keywords.TryGetValue(lexeme, out var type))
                {
                    return new Token
                    {
                        Column = first.Column,
                        Line = first.Line,
                        Lexeme = lexeme,
                        Kind = type
                    };
                }

                return new Token
                {
                    Column = first.Column,
                    Line = first.Line,
                    Lexeme = lexeme,
                    Kind = TokenKind.Identifier,
                    Value = lexeme
                };
            }

            var next = codeCursor.Consume();
            strBuilder.Append(next.Char);
        }

        var msg = $"Identifier starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
                  $"maximum length of {MaxSize} characters.";
        errors?.Add(new SyntaxError(first, msg));
        return null;
    }

    #endregion
}