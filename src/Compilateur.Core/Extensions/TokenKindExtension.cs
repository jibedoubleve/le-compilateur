using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Extensions;

public static class TokenKindExtension
{
    #region Methods

    public static string ToLexeme(this TokenKind kind) =>
        kind switch
        {
            TokenKind.Dot                => ".",
            TokenKind.Comma              => ",",
            TokenKind.Semicolon          => ";",
            TokenKind.OpenParenthesis    => "(",
            TokenKind.CloseParenthesis   => ")",
            TokenKind.OpenCurlyBracket   => "{",
            TokenKind.CloseCurlyBracket  => "}",
            TokenKind.Bang               => "!",
            TokenKind.GreaterThan        => ">",
            TokenKind.LessThan           => "<",
            TokenKind.Assignment         => "=",
            TokenKind.Plus               => "+",
            TokenKind.Minus              => "-",
            TokenKind.Multiply           => "*",
            TokenKind.Divided            => "/",
            TokenKind.And                => "and",
            TokenKind.Or                 => "or",
            TokenKind.GreaterThanOrEqual => ">=",
            TokenKind.LessThanOrEqual    => "<=",
            TokenKind.Equality           => "==",
            TokenKind.Inequality         => "!=",
            TokenKind.Nil                => "nil",
            TokenKind.If                 => "if",
            TokenKind.Else               => "else",
            TokenKind.While              => "while",
            TokenKind.For                => "for",
            TokenKind.Fun                => "fun",
            TokenKind.Return             => "return",
            TokenKind.Class              => "class",
            TokenKind.This               => "this",
            TokenKind.Super              => "super",
            TokenKind.Var                => "var",
            TokenKind.Print              => "print",
            TokenKind.Eof                => "EOF",
            TokenKind.False              => "false",
            TokenKind.True               => "true",
            TokenKind.Numeric            => "numeric",
            TokenKind.Identifier         => "identifier",
            TokenKind.String             => "string",
            _ => throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                $"Symbol token {kind} is not supported.")
        };

    #endregion
}