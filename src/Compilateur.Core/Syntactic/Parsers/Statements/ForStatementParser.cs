using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Statements;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class ForStatementParser : Parser<StatementNode>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();
    private static readonly StatementParser StatementParser = new();

    #endregion

    #region Methods

    protected override StatementNode? ParseCore(ParsingContext context)
    {
        var @for = context.Cursor.Consume(); // consume the for
        if (!context.Cursor.IsPeekOfKind(TokenKind.OpenParenthesis))
        {
            context.AddError($"Expected parenthesis but found '{context.Cursor.Peek().Lexeme}'");
            return null;
        }

        context.Cursor.Consume(); // Consume '('

        if (!context.TryParseInitialiser(out var initialiser)) { return null; }

        if (!context.TryParseCondition(out var condition)) { return null; }

        ExpressionNode? increment = null;
        if (!context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis))
        {
            increment = ExpressionParser.Parse(context);
            if (increment is null) { return null; }
        }

        /* Closing parenthesis
         */
        if (!context.Cursor.IsPeekOfKind(TokenKind.CloseParenthesis))
        {
            context.AddError("No closing parenthesis found");
            return null;
        }

        context.Cursor.Consume(); // Consume the ')'
        var body = StatementParser.Parse(context);
        if (body is null) { return null; }

        var statements = new List<StatementNode>();
        if (initialiser is not null) { statements.Add(initialiser); }

        var whileInstructions = new List<StatementNode> { body };
        if (increment is not null) { whileInstructions.Add(new ExpressionStatement(increment)); }

        statements.Add(new WhileStatement(@for, condition, new BlockStatement(@for, whileInstructions)));

        return new BlockStatement(@for, statements);
    }

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.For);

    #endregion
}