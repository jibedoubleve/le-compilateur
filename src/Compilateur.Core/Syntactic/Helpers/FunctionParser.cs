using Compilateur.Core.Lexical.Tokens;

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

    private static IEnumerable<SyntaxNode>? ParseArguments(ParsingContext context)
    {
        if (context.Cursor.IsPeekOfType(TokenType.OpenParenthesis))
        {
            context.Cursor.Consume(); // Consume '('
            var arguments = new List<SyntaxNode>();

            var current = context.Cursor.Peek();
            switch (current.Type)
            {
                case TokenType.CloseParenthesis:
                    context.Cursor.Consume();
                    return [];
                case TokenType.Identifier:
                    arguments.Add(SyntaxNode.Argument(current));
                    context.Cursor.Consume();
                    break;
            }

            while (current.Type != TokenType.Eof)
            {
                current = context.Cursor.Peek();

                switch (current.Type)
                {
                    case TokenType.Comma:
                        context.Cursor.Consume();
                        break;
                    case TokenType.CloseParenthesis:
                        context.Cursor.Consume();
                        return ValidateArguments(arguments, context);
                    case TokenType.Identifier:
                        context.Cursor.Consume();
                        arguments.Add(SyntaxNode.Argument(current));
                        break;
                    default:
                        context.AddError(
                            $"Expect identifier while found '{current.Lexeme}'."
                        );
                        return null;
                }
            }

            return ValidateArguments(arguments, context);
        }

        context.AddError("Expected '(' after function name.");
        return null;
    }

    private static IEnumerable<SyntaxNode>? ValidateArguments(IEnumerable<SyntaxNode> arguments, ParsingContext context)
    {
        var args = arguments as SyntaxNode[] ?? [.. arguments];
        if (args.Length <= MaxParams) { return args; }

        context.AddError($"Maximum number of {MaxParams} parameters exceeded.");
        return null;
    }

    public static SyntaxNode? Parse(ParsingContext context)
    {
        var funcName = context.Cursor.Peek(); // Consume the identifier
        if (funcName.Type != TokenType.Identifier)
        {
            context.AddError(
                $"Expected a function identifier but found '{funcName.Lexeme}' [{funcName.Type}]"
            );
            return null;
        }

        context.Cursor.Consume(); // Consume the identifier

        var arguments = ParseArguments(context);
        if (arguments is null)
        {
            context.AddError(
                $"Failed to parse arguments of function '{funcName.Lexeme}' - see the error above."
            );
            return null;
        }

        var block = BlockParser.Parse(context);
        if (block is null)
        {
            context.AddError(
                $"Failed to parse function '{funcName.Lexeme}' - see the error above."
            );
            return null;
        }

        return SyntaxNode.Function(funcName, [.. arguments, SyntaxNode.Body(block)]);
    }

    #endregion
}