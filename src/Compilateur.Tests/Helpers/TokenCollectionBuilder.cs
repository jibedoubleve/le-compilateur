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
        if (!_tokens.Any() || _tokens.Last().Type != TokenType.Eof)
        {
            _tokens.Add(new Token
            {
                Column = 0,
                Line = 0,
                Lexeme = "$",
                Type = TokenType.Eof
            });
        }

        return new TokenCursor(_tokens);
    }

    private static TokenCursor BuildEmpty() => new([]);

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

    public TokenCollectionBuilder And() => Symbol(TokenType.And);

    public TokenCollectionBuilder Bang() => Symbol(TokenType.Bang);

    public TokenCollectionBuilder BetweenCurlyBracket(Action<TokenCollectionBuilder>? expression = null)
    {
        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = "{",
            Type = TokenType.OpenCurlyBracket
        });

        expression?.Invoke(this);

        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = "}",
            Type = TokenType.CloseCurlyBracket
        });
        return this;
    }

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

    public static ParsingContext BuildEmptyParsingContext() => new(BuildEmpty());

    public ParsingContext BuildParsingContext() => new(BuildCursor());

    public TokenCollectionBuilder Class(string? className, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenType.Class);
        if (!string.IsNullOrEmpty(className))
        {
            builder.Identifier(className);
        }

        builder.Symbol(TokenType.OpenCurlyBracket);
        expression?.Invoke(this);
        builder.Symbol(TokenType.CloseCurlyBracket);

        return builder;
    }

    public TokenCollectionBuilder CloseCurlyBracket() => Symbol(TokenType.CloseCurlyBracket);
    public TokenCollectionBuilder CloseParenthesis() => Symbol(TokenType.CloseParenthesis);
    public TokenCollectionBuilder Comma() => Symbol(TokenType.Comma);
    public TokenCollectionBuilder Divided() => Symbol(TokenType.Divided);

    public TokenCollectionBuilder Dot() => Symbol(TokenType.Dot);

    public TokenCollectionBuilder DoubleEqual() => Symbol(TokenType.Equality);
    public TokenCollectionBuilder Else() => Symbol(TokenType.Else);

    /// <summary>
    ///     Adds the tokens for an empty argument list, i.e. '()'.
    /// </summary>
    public TokenCollectionBuilder EmptyCall() =>
        Symbol(TokenType.OpenParenthesis)
            .Symbol(TokenType.CloseParenthesis);

    public TokenCollectionBuilder Eof()
    {
        Symbol(TokenType.Eof);
        return this;
    }

    public TokenCollectionBuilder Equal() => Symbol(TokenType.Assignment);

    public TokenCollectionBuilder For() => Symbol(TokenType.For);

    public TokenCollectionBuilder Fun() => Symbol(TokenType.Fun);

    public TokenCollectionBuilder Fun(string? functionName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Fun();
        if (!string.IsNullOrEmpty(functionName))
        {
            builder.Identifier(functionName);
        }

        builder.Symbol(TokenType.OpenParenthesis);
        expression?.Invoke(this);
        builder.Symbol(TokenType.CloseParenthesis);

        return builder;
    }

    public TokenCollectionBuilder GreaterThan() => Symbol(TokenType.GreaterThan);
    public TokenCollectionBuilder GreaterThanOrEqual() => Symbol(TokenType.GreaterThanOrEqual);
    public TokenCollectionBuilder Identifier(string name) => Symbol(TokenType.Identifier, name);
    public TokenCollectionBuilder If() => Symbol(TokenType.If);
    public TokenCollectionBuilder Inequality() => Symbol(TokenType.Inequality);
    public TokenCollectionBuilder LessThan() => Symbol(TokenType.LessThan);
    public TokenCollectionBuilder LessThanOrEqual() => Symbol(TokenType.LessThanOrEqual);
    public TokenCollectionBuilder Minus() => Symbol(TokenType.Minus);
    public TokenCollectionBuilder Multiply() => Symbol(TokenType.Multiply);

    public TokenCollectionBuilder Nil()
    {
        Symbol(TokenType.Nil);
        return this;
    }

    public TokenCollectionBuilder Number(double number) => Value(TokenType.Numeric, number);
    public TokenCollectionBuilder Number(string number) => Value(TokenType.Numeric, number);

    public TokenCollectionBuilder OpenCurlyBracket() => Symbol(TokenType.OpenCurlyBracket);

    public TokenCollectionBuilder OpenParenthesis() => Symbol(TokenType.OpenParenthesis);
    public TokenCollectionBuilder Or() => Symbol(TokenType.Or);
    public TokenCollectionBuilder Plus() => Symbol(TokenType.Plus);

    public TokenCollectionBuilder Print(string output)
    {
        Symbol(TokenType.Print).String(output);
        return this;
    }

    public TokenCollectionBuilder Return() => Symbol(TokenType.Return);
    public TokenCollectionBuilder Semicolon() => Symbol(TokenType.Semicolon);

    public TokenCollectionBuilder String(string output)
    {
        Symbol(TokenType.String, output);
        return this;
    }

    public TokenCollectionBuilder Subclass(
        string className, string subclassName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenType.Class)
                      .Identifier(className)
                      .Symbol(TokenType.LessThan)
                      .Identifier(subclassName);

        builder.Symbol(TokenType.OpenCurlyBracket);
        expression?.Invoke(this);
        builder.Symbol(TokenType.CloseCurlyBracket);

        return builder;
    }


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
            TokenType.Eof                => "$",
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

    public TokenCollectionBuilder True()
    {
        Symbol(TokenType.True);
        return this;
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
        var builder = Symbol(TokenType.Var)
            .Identifier(variableName);

        if (expression != null)
        {
            builder.Symbol(TokenType.Assignment);
            expression.Invoke(this);
        }

        return this;
    }

    public TokenCollectionBuilder While() => Symbol(TokenType.While);

    #endregion
}