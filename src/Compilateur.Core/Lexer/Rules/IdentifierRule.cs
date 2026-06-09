using System.Text;
using Compilateur.Core.Lexer.Tokens;

namespace Compilateur.Core.Lexer.Rules;

public sealed record IdentifierRule : ITokenRule
{
    #region Fields

    private readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "and", TokenType.And },
        { "or", TokenType.Or },
        { "nil", TokenType.Nil },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "var", TokenType.Var },
        { "class", TokenType.Class },
        { "this", TokenType.This },
        { "super", TokenType.Super },
        { "return", TokenType.Return },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "print", TokenType.Print }
    };


    private const int MaxSize = 10_000;

    #endregion

    #region Properties

    public int Weight => 999;

    #endregion

    #region Methods

    private bool IsValidChar(CodeStream codeStream)
    {
        if (codeStream.IsEof) return false;

        char? character = codeStream.Peek();
        return character.HasValue &&
               (char.IsAsciiLetterOrDigit(character.Value) || character == '_');
    }

    public bool Matches(CodeStream codeStream)
    {
        if (codeStream.IsEof) return false;

        char? character = codeStream.Peek();
        return character.HasValue &&
               (char.IsAsciiLetter(character.Value) || character == '_');
    }

    public Token? Scan(CodeStream codeStream, SyntaxErrorCollection? errors = null)
    {
        var first = codeStream.Consume();

        var strBuilder = new StringBuilder();
        strBuilder.Append(first.Char);

        for (var i = 0; i < MaxSize; i++)
        {
            if (!IsValidChar(codeStream))
            {
                var lexeme = strBuilder.ToString();
                if (_keywords.TryGetValue(lexeme, out var type))
                    return new Token
                    {
                        Column = first.Column,
                        Line = first.Line,
                        Lexeme = lexeme,
                        Type = type
                    };

                return new Token
                {
                    Column = first.Column,
                    Line = first.Line,
                    Lexeme = lexeme,
                    Type = TokenType.Identifier,
                    Value = lexeme
                };
            }

            var next = codeStream.Consume();
            strBuilder.Append(next.Char);
        }

        errors?.Add(new SyntaxError
        {
            Column = first.Column,
            Line = first.Line,
            Message =
                $"Identifier starting with '{first.Char}' at line {first.Line}, column {first.Column} exceeds the " +
                $"maximum length of {MaxSize} characters."
        });
        return null;
    }

    #endregion
}