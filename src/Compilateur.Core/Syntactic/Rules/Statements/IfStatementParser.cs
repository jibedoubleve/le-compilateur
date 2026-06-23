using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class IfStatementParser : IParser
{
    #region Fields

    private static readonly BlockStatementParser BlockStatementParser = new();

    private static readonly ExpressionParser ExpressionParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.If);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError($"Expected 'if' but specified '{context.Cursor.Peek().Type}'.");
            return null;
        }

        var keyword = SyntaxNode.Unspecified(context.Cursor.Consume());
        var blocks = new List<SyntaxNode>();
        
        // Handle the IF condition
        var expression = ExpressionParser.Parse(context);
        if (expression is null)
        {
            context.AddError($"Expected ')' after expression but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        blocks.Add(SyntaxNode.Condition(expression));

        // Handle the IF block
        var ifBlock = BlockStatementParser.Parse(context);
        if (ifBlock is null)
        {
            context.AddError($"Expected a block after a 'if' statement but found '{context.Cursor.Peek().Lexeme}'.");
            return null;
        }

        blocks.Add(SyntaxNode.Then(ifBlock));

        // Handle the ELSE block
        if (context.Cursor.IsPeekOfType(TokenType.Else))
        {
            context.Cursor.Consume(); // consume the 'else'...
            var elseBlock = BlockStatementParser.Parse(context);
            if (elseBlock is null)
            {
                context.AddError(
                    $"Expected a block after an 'else' statement but found '{context.Cursor.Peek().Lexeme}'.");
                return null;
            }

            blocks.Add(SyntaxNode.Else(elseBlock));
        }

        // Build the node
        return SyntaxNode.Unspecified(keyword.Token, [.. blocks]);
    }

    #endregion
}