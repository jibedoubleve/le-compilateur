using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;

namespace Compilateur.Tests.Helpers;

public sealed class TokenCollectionBuilder
{
    #region Fields

    private readonly List<Token> _tokens = [];

    #endregion

    #region Methods

    private TokenCursor BuildCursor()
    {
        if (_tokens.Last().Type != TokenType.Eof)
        {
            _tokens.Add(new Token
            {
                Column = 0,
                Line = 0,
                Lexeme = "",
                Type = TokenType.Eof
            });
        }

        return new TokenCursor(_tokens);
    }

    private TokenCollectionBuilder Symbol(TokenType tokenType, string lexeme)
    {
        _tokens.Add(new Token
        {
            Type = tokenType,
            Lexeme = $"{lexeme}",
            Value = null,
            Column = 0,
            Line = 0
        });
        return this;
    }

    public TokenCollectionBuilder And() => Symbol(TokenType.And, "and");

    public TokenCollectionBuilder Bang() => Symbol(TokenType.Bang, "!");

    public TokenCollectionBuilder BetweenParentheses(Action<TokenCollectionBuilder> expression)
    {
        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = "(",
            Type = TokenType.OpenParenthesis
        });

        expression(this);

        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = ")",
            Type = TokenType.CloseParenthesis
        });
        return this;
    }

    public ParsingContext BuildParsingContext() => new(BuildCursor());
    public TokenCollectionBuilder Comma() => Symbol(TokenType.Comma, ",");
    public TokenCollectionBuilder Divided() => Symbol(TokenType.Divided, "/");
    public TokenCollectionBuilder Else() => Symbol(TokenType.Else, "else");

    /// <summary>
    ///     Adds the tokens for an empty argument list, i.e. '()'.
    /// </summary>
    public TokenCollectionBuilder EmptyCall() =>
        Symbol(TokenType.OpenParenthesis, "(")
            .Symbol(TokenType.CloseParenthesis, ")");

    public TokenCollectionBuilder Equal() => Symbol(TokenType.Assignment, "=");

    public TokenCollectionBuilder Equality() => Symbol(TokenType.Equality, "==");
    public TokenCollectionBuilder For() => Symbol(TokenType.For, "for");
    public TokenCollectionBuilder GreaterThan() => Symbol(TokenType.GreaterThan, ">");
    public TokenCollectionBuilder GreaterThanOrEqual() => Symbol(TokenType.GreaterThanOrEqual, ">=");
    public TokenCollectionBuilder Identifier(string name) => Symbol(TokenType.Identifier, name);
    public TokenCollectionBuilder If() => Symbol(TokenType.If, "if");
    public TokenCollectionBuilder Inequality() => Symbol(TokenType.Inequality, "!=");
    public TokenCollectionBuilder LessThan() => Symbol(TokenType.LessThan, "<");
    public TokenCollectionBuilder LessThanOrEqual() => Symbol(TokenType.LessThanOrEqual, "<=");
    public TokenCollectionBuilder Minus() => Symbol(TokenType.Minus, "-");
    public TokenCollectionBuilder Multiply() => Symbol(TokenType.Multiply, "*");
    public TokenCollectionBuilder Number(double number) => Value(TokenType.Numeric, number);
    public TokenCollectionBuilder Or() => Symbol(TokenType.Or, "or");
    public TokenCollectionBuilder Plus() => Symbol(TokenType.Plus, "+");
    public TokenCollectionBuilder Return() => Symbol(TokenType.Return, "return");
    public TokenCollectionBuilder Semicolon() => Symbol(TokenType.Semicolon, ";");

    public TokenCollectionBuilder Symbol(TokenType tokenType)
    {
        var lexeme = tokenType switch
        {
            TokenType.Dot                => ".",
            TokenType.Comma              => ",",
            TokenType.Semicolon          => ";",
            TokenType.OpenParenthesis    => "(",
            TokenType.CloseParenthesis   => ")",
            TokenType.OpenCurlyBracket   => "{",
            TokenType.CloseCurlyBracket  => "}",
            TokenType.Bang               => "!",
            TokenType.GreaterThan        => ">",
            TokenType.LessThan           => "<",
            TokenType.Assignment         => "=",
            TokenType.Plus               => "+",
            TokenType.Minus              => "-",
            TokenType.Multiply           => "*",
            TokenType.Divided            => "/",
            TokenType.And                => "and",
            TokenType.Or                 => "or",
            TokenType.GreaterThanOrEqual => ">=",
            TokenType.LessThanOrEqual    => "<=",
            TokenType.Equality           => "==",
            TokenType.Inequality         => "!=",
            TokenType.Nil                => "nil",
            TokenType.If                 => "if",
            TokenType.Else               => "else",
            TokenType.While              => "while",
            TokenType.For                => "for",
            TokenType.Fun                => "fun",
            TokenType.Return             => "return",
            TokenType.Class              => "class",
            TokenType.This               => "this",
            TokenType.Super              => "super",
            TokenType.Var                => "var",
            TokenType.Print              => "print",
            TokenType.Eof                => "eof",
            TokenType.False              => "false",
            TokenType.True               => "true",
            TokenType.Numeric            => string.Empty,
            TokenType.Identifier         => string.Empty,
            TokenType.String             => string.Empty,
            _ => throw new ArgumentOutOfRangeException(
                nameof(tokenType),
                tokenType,
                $"Symbol token {tokenType} is not supported.")
        };
        return Symbol(tokenType, lexeme);
    }

    public TokenCollectionBuilder Value(TokenType tokenType, object? value)
    {
        _tokens.Add(new Token
        {
            Type = tokenType,
            Lexeme = $"{value}",
            Value = value,
            Column = 0,
            Line = 0
        });
        return this;
    }

    public TokenCollectionBuilder Var(string variableName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenType.Var, "var")
            .Symbol(TokenType.Identifier, variableName);

        if (expression != null)
        {
            builder.Symbol(TokenType.Assignment);
            expression.Invoke(this);
        }

        return this;
    }

    public TokenCollectionBuilder While() => Symbol(TokenType.While, "while");

    #endregion
}