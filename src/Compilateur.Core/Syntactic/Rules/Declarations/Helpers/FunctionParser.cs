using Compilateur.Core.Errors.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Declarations.Helpers;

/// <summary>
///     Base parser for function declarations, shared by both a method
///     declared inside a class (<c>class MyClass { MyFun() {...} }</c>)
///     and a top-level function declaration (<c>fun MyFun() {...}</c>).
/// </summary>
public static class FunctionParser
{
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
                        return arguments;
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

            return arguments;
        }

        context.AddError("Expected '(' after function name.");
        return null;
    }

    private static IEnumerable<SyntaxNode>? ParseBlock(ParsingContext context)
    {
        if (context.Cursor.IsPeekOfType(TokenType.OpenCurlyBracket))
        {
            var expressionParser = new ExpressionParser();
            context.Cursor.Consume(); // Consume the '{'
            var expressions = new List<SyntaxNode>();

            while (true)
            {
                if (context.Cursor.IsPeekOfType(TokenType.CloseCurlyBracket))
                {
                    context.Cursor.Consume();
                    break;
                }

                var node = expressionParser.Parse(context);
                if (node == null)
                {
                    return null;
                }

                expressions.Add(SyntaxNode.Declaration(node));

                var cur = context.Cursor.Peek();
                if (cur.Type != TokenType.Semicolon)
                {
                    context.AddError($"Expected ';' but found '{cur.Lexeme}'.");
                    return null;
                }

                context.Cursor.Consume();
            }

            return expressions;
        }

        context.AddError("Expected '{' before function body.");
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

        var declarations = ParseBlock(context);
        if (declarations is null)
        {
            context.AddError(
                $"Failed to parse function '{funcName.Lexeme}' - see the error above."
            );
            return null;
        }

        return SyntaxNode.Function(funcName, [.. arguments, .. declarations]);
    }

    #endregion
}