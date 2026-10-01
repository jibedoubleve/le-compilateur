using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;

namespace Compilateur.Core.Syntactic.Helpers;

/// <summary>
///     Base parser for function declarations, shared by both a method
///     declared inside a class (<c>class MyClass { MyFun() {...} }</c>)
///     and a top-level function declaration (<c>fun MyFun() {...}</c>).
/// </summary>
public static class FunctionParser
{
    #region Fields

    private const int MaxParams = 255;

    #endregion

    #region Methods

    private static IEnumerable<Token>? ParseParameters(ParsingContext context)
    {
        if (context.Cursor.IsPeekOfKind(TokenKind.OpenParenthesis))
        {
            context.Cursor.Consume(); // Consume '('
            var parameters = new List<Token>();

            var current = context.Cursor.Consume();
            switch (current.Kind)
            {
                case TokenKind.CloseParenthesis:
                    return [];
                case TokenKind.Identifier:
                    parameters.Add(current);
                    if (!ValidateParameterSeparator(context)) { return null; }

                    break;
                default:
                    context.AddError(
                        $"Expected identifier, found '{current.Lexeme}'."
                    );
                    return null;
            }

            while (current.Kind != TokenKind.Eof)
            {
                current = context.Cursor.Consume();

                switch (current.Kind)
                {
                    case TokenKind.Comma:
                        if (!context.Cursor.IsPeekOfKind(TokenKind.Identifier))
                        {
                            context.AddError($"Expected identifier, found '{context.Cursor.Peek().Lexeme}'.");
                            return null;
                        }

                        break;
                    case TokenKind.CloseParenthesis:
                        return ValidateParameters(parameters, context);
                    case TokenKind.Identifier:
                        if (!ValidateParameterSeparator(context)) { return null; }

                        parameters.Add(current);
                        break;
                    default:
                        context.AddError(
                            $"Expected identifier, found '{current.Lexeme}'."
                        );
                        return null;
                }
            }

            return ValidateParameters(parameters, context);
        }

        context.AddError("Expected '(' after function name.");
        return null;
    }

    private static bool ValidateParameterSeparator(ParsingContext context)
    {
        if (context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis, TokenKind.Comma)) { return true; }

        context.AddError($"Expected identifier, ',' or ')', found '{context.Cursor.Peek().Lexeme}'.");
        return false;
    }

    private static IEnumerable<Token>? ValidateParameters(
        IEnumerable<Token> parameters, ParsingContext context)
    {
        var args = parameters as Token[] ?? [.. parameters];
        if (args.Length <= MaxParams) { return args; }

        context.AddError($"Maximum number of {MaxParams} parameters exceeded.");
        return null;
    }

    public static FunctionDeclarationStatement? Parse(ParsingContext context)
    {
        var funcName = context.Cursor.Peek(); // Consume the identifier
        if (funcName.Kind != TokenKind.Identifier)
        {
            context.AddError(
                $"Expected a function identifier, found '{funcName.Lexeme}' [{funcName.Kind}]"
            );
            return null;
        }

        context.Cursor.Consume(); // Consume the identifier

        var parameters = ParseParameters(context);
        if (parameters is null) { return null; }

        var block = BlockParser.Parse(context);
        if (block is null) { return null; }

        return new FunctionDeclarationStatement(funcName, parameters ?? [], block);
    }

    #endregion
}