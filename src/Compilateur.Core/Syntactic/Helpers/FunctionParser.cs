using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Statements;

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

    private static IEnumerable<ParameterNode>? ParseParameters(ParsingContext context)
    {
        if (context.Cursor.IsPeekOfKind(TokenKind.OpenParenthesis))
        {
            context.Cursor.Consume(); // Consume '('
            var parameters = new List<ParameterNode>();

            var current = context.Cursor.Peek();
            switch (current.Kind)
            {
                case TokenKind.CloseParenthesis:
                    context.Cursor.Consume();
                    return [];
                case TokenKind.Identifier:
                    parameters.Add(new ParameterNode(current));
                    context.Cursor.Consume();
                    break;
            }

            while (current.Kind != TokenKind.Eof)
            {
                current = context.Cursor.Peek();

                switch (current.Kind)
                {
                    case TokenKind.Comma:
                        context.Cursor.Consume();
                        break;
                    case TokenKind.CloseParenthesis:
                        context.Cursor.Consume();
                        return ValidateParameters(parameters, context);
                    case TokenKind.Identifier:
                        context.Cursor.Consume();
                        parameters.Add(new ParameterNode(current));
                        break;
                    default:
                        context.AddError(
                            $"Expect identifier while found '{current.Lexeme}'."
                        );
                        return null;
                }
            }

            return ValidateParameters(parameters, context);
        }

        context.AddError("Expected '(' after function name.");
        return null;
    }

    private static IEnumerable<ParameterNode>? ValidateParameters(
        IEnumerable<ParameterNode> parameters, ParsingContext context)
    {
        var args = parameters as ParameterNode[] ?? [.. parameters];
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
        if (parameters is null)
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

        return new FunctionDeclarationStatement(funcName, parameters ?? [], block);
    }

    #endregion
}