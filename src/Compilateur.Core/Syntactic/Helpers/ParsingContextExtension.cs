using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Parsers;
using Compilateur.Core.Syntactic.Parsers.Declarations;
using Compilateur.Core.Syntactic.Parsers.Expressions;
using Compilateur.Core.Syntactic.Parsers.Statements;

namespace Compilateur.Core.Syntactic.Helpers;

public static class ParsingContextExtension
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    private static readonly ExpressionStatementParser ExpressionStatementParser = new();
    private static readonly VarDeclarationParser VarDeclarationParser = new();

    #endregion

    /// <param name="context">The parsing context.</param>
    extension(ParsingContext context)
    {
        #region Methods

        public bool ConsumeOrError(TokenKind kind)
        {
            if (!context.Cursor.IsPeekOfKind(kind))
            {
                context.AddError($"Expected '{kind.ToLexeme()}' but found '{context.Cursor.Peek().Lexeme}'");
                return false;
            }

            context.Cursor.Consume();
            return true;
        }

        public bool TryParseCondition(out ExpressionNode? output)
        {
            if (context.Cursor.IsPeekOfKind(TokenKind.Semicolon))
            {
                context.Cursor.Consume(); // Consume the ';'
                output = null;
                return true;
            }

            var expr = context.ValidateFinalSemicolon(
                ExpressionParser.Parse(context)
            );
            if (expr is null)
            {
                output = null;
                return false;
            }

            output = expr;
            return true;
        }

        public bool TryParseInitialiser(out StatementNode? output)
        {
            if (context.Cursor.IsPeekOfKind(TokenKind.Semicolon))
            {
                context.Cursor.Consume(); // Consume the ';'
                output = null;
                return true;
            }

            if (context.Cursor.IsPeekOfKind(TokenKind.Var))
            {
                var decl = VarDeclarationParser.Parse(context);
                if (decl is null)
                {
                    output = null;
                    return false;
                }

                output = decl;
                return true;
            }

            var statement = ExpressionStatementParser.Parse(context);
            if (statement is null)
            {
                output = null;
                return false;
            }

            output = statement;
            return true;
        }

        /// <summary>
        ///     Validates that the current token is a semicolon terminating a statement.
        ///     If the check fails, an error is recorded in the parsing context.
        /// </summary>
        /// <param name="node">The node to return unchanged if validation succeeds.</param>
        /// <returns><paramref name="node" /> if the current token is a semicolon; otherwise <c>null</c>.</returns>
        public T? ValidateFinalSemicolon<T>(T? node)
            where T : SyntaxNode
        {
            if (context.Cursor.IsPeekOfKind(TokenKind.Semicolon))
            {
                context.Cursor.Consume(); // Consume the ';'
                return node;
            }

            context.AddError($"Expected ';' but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        #endregion
    }
}