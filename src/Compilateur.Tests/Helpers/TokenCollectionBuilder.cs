using Compilateur.Core.Extensions;
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
        if (!_tokens.Any() || _tokens.Last().Kind != TokenKind.Eof)
        {
            _tokens.Add(new Token
            {
                Column = 0,
                Line = 0,
                Lexeme = "$",
                Kind = TokenKind.Eof
            });
        }

        return new TokenCursor(_tokens);
    }

    private static TokenCursor BuildEmpty() => new([]);

    private TokenCollectionBuilder Symbol(TokenKind tokenKind, string lexeme)
    {
        _tokens.Add(new Token
        {
            Kind = tokenKind,
            Lexeme = $"{lexeme}",
            Value = null,
            Column = 0,
            Line = 0
        });
        return this;
    }

    public TokenCollectionBuilder And() => Symbol(TokenKind.And);

    public TokenCollectionBuilder Bang() => Symbol(TokenKind.Bang);

    public TokenCollectionBuilder BetweenCurlyBracket(Action<TokenCollectionBuilder>? expression = null)
    {
        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = "{",
            Kind = TokenKind.OpenCurlyBracket
        });

        expression?.Invoke(this);

        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = "}",
            Kind = TokenKind.CloseCurlyBracket
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
            Kind = TokenKind.OpenParenthesis
        });

        expression(this);

        _tokens.Add(new Token
        {
            Column = 0,
            Line = 0,
            Lexeme = ")",
            Kind = TokenKind.CloseParenthesis
        });
        return this;
    }

    public static ParsingContext BuildEmptyParsingContext() => new(BuildEmpty());

    public ParsingContext BuildParsingContext() => new(BuildCursor());

    public TokenCollectionBuilder Class(string? className, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenKind.Class);
        if (!string.IsNullOrEmpty(className))
        {
            builder.Identifier(className);
        }

        builder.Symbol(TokenKind.OpenCurlyBracket);
        expression?.Invoke(this);
        builder.Symbol(TokenKind.CloseCurlyBracket);

        return builder;
    }

    public TokenCollectionBuilder CloseCurlyBracket() => Symbol(TokenKind.CloseCurlyBracket);
    public TokenCollectionBuilder CloseParenthesis() => Symbol(TokenKind.CloseParenthesis);
    public TokenCollectionBuilder Comma() => Symbol(TokenKind.Comma);
    public TokenCollectionBuilder Divided() => Symbol(TokenKind.Divided);

    public TokenCollectionBuilder Dot() => Symbol(TokenKind.Dot);

    public TokenCollectionBuilder DoubleEqual() => Symbol(TokenKind.Equality);
    public TokenCollectionBuilder Else() => Symbol(TokenKind.Else);

    /// <summary>
    ///     Adds the tokens for an empty argument list, i.e. '()'.
    /// </summary>
    public TokenCollectionBuilder EmptyCall() =>
        Symbol(TokenKind.OpenParenthesis)
            .Symbol(TokenKind.CloseParenthesis);

    public TokenCollectionBuilder Eof()
    {
        Symbol(TokenKind.Eof);
        return this;
    }

    public TokenCollectionBuilder Equal() => Symbol(TokenKind.Assignment);

    public TokenCollectionBuilder For() => Symbol(TokenKind.For);

    public TokenCollectionBuilder Fun() => Symbol(TokenKind.Fun);

    public TokenCollectionBuilder Fun(string? functionName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Fun();
        if (!string.IsNullOrEmpty(functionName))
        {
            builder.Identifier(functionName);
        }

        builder.Symbol(TokenKind.OpenParenthesis);
        expression?.Invoke(this);
        builder.Symbol(TokenKind.CloseParenthesis);

        return builder;
    }

    public TokenCollectionBuilder GreaterThan() => Symbol(TokenKind.GreaterThan);
    public TokenCollectionBuilder GreaterThanOrEqual() => Symbol(TokenKind.GreaterThanOrEqual);
    public TokenCollectionBuilder Identifier(string name) => Symbol(TokenKind.Identifier, name);
    public TokenCollectionBuilder If() => Symbol(TokenKind.If);
    public TokenCollectionBuilder Inequality() => Symbol(TokenKind.Inequality);
    public TokenCollectionBuilder LessThan() => Symbol(TokenKind.LessThan);
    public TokenCollectionBuilder LessThanOrEqual() => Symbol(TokenKind.LessThanOrEqual);
    public TokenCollectionBuilder Minus() => Symbol(TokenKind.Minus);
    public TokenCollectionBuilder Multiply() => Symbol(TokenKind.Multiply);

    public TokenCollectionBuilder Nil()
    {
        Symbol(TokenKind.Nil);
        return this;
    }

    public TokenCollectionBuilder Number(double number) => Value(TokenKind.Numeric, number);
    public TokenCollectionBuilder Number(string number) => Value(TokenKind.Numeric, number);

    public TokenCollectionBuilder OpenCurlyBracket() => Symbol(TokenKind.OpenCurlyBracket);

    public TokenCollectionBuilder OpenParenthesis() => Symbol(TokenKind.OpenParenthesis);
    public TokenCollectionBuilder Or() => Symbol(TokenKind.Or);
    public TokenCollectionBuilder Plus() => Symbol(TokenKind.Plus);

    public TokenCollectionBuilder Print(string output)
    {
        Symbol(TokenKind.Print).String(output);
        return this;
    }

    public TokenCollectionBuilder Return() => Symbol(TokenKind.Return);
    public TokenCollectionBuilder Semicolon() => Symbol(TokenKind.Semicolon);

    public TokenCollectionBuilder String(string output)
    {
        Symbol(TokenKind.String, output);
        return this;
    }

    public TokenCollectionBuilder Subclass(
        string className, string subclassName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenKind.Class)
                      .Identifier(className)
                      .Symbol(TokenKind.LessThan)
                      .Identifier(subclassName);

        builder.Symbol(TokenKind.OpenCurlyBracket);
        expression?.Invoke(this);
        builder.Symbol(TokenKind.CloseCurlyBracket);

        return builder;
    }


    public TokenCollectionBuilder Symbol(TokenKind tokenKind) => Symbol(tokenKind, tokenKind.ToLexeme());

    public TokenCollectionBuilder True()
    {
        Symbol(TokenKind.True);
        return this;
    }

    public TokenCollectionBuilder Value(TokenKind tokenKind, object? value)
    {
        _tokens.Add(new Token
        {
            Kind = tokenKind,
            Lexeme = $"{value}",
            Value = value,
            Column = 0,
            Line = 0
        });
        return this;
    }

    public TokenCollectionBuilder Var(string variableName, Action<TokenCollectionBuilder>? expression = null)
    {
        var builder = Symbol(TokenKind.Var)
            .Identifier(variableName);

        if (expression != null)
        {
            builder.Symbol(TokenKind.Assignment);
            expression.Invoke(this);
        }

        return this;
    }

    public TokenCollectionBuilder While() => Symbol(TokenKind.While);

    #endregion
}