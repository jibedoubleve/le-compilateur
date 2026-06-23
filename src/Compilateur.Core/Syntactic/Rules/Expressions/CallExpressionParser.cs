using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class CallExpressionParser : PrecedenceParser<PrimaryExpressionParser>
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();

    private static readonly TokenType[] PostfixOperators = [TokenType.OpenParenthesis, TokenType.Dot];

    #endregion

    #region Methods

    private static SyntaxNode[] GetArguments(ParsingContext context)
    {
        var children = new List<SyntaxNode>();

        if (context.Cursor.IsPeekOfType(TokenType.OpenParenthesis))
        {
            context.Cursor.Consume();
        }

        while (!context.Cursor.IsAtEnd && !context.Cursor.IsPeekOfType(TokenType.CloseParenthesis))
        {
            if (!ExpressionParser.Matches(context)) { break; }

            var node = ExpressionParser.Parse(context);
            if (node is not null)
            {
                children.Add(node);
            }

            if (context.Cursor.IsPeekOfType(TokenType.Comma))
            {
                context.Cursor.Consume();
            }
        }

        if (!context.Cursor.IsAtEnd)
        {
            context.Cursor.Consume(); // Drop the ')'
        }

        return [.. children];
    }

    protected override bool MatchesCurrent(ParsingContext context) => false;

    public override SyntaxNode? Parse(ParsingContext context)
    {
        var node = InnerExpression.Parse(context);
        var current = context.Cursor.Peek();

        if (node is null || !PostfixOperators.Contains(current.Type))
        {
            return node;
        }

        current = context.Cursor.Consume(); // Consume the "(" or the "."
        var accumulator = current.IsOfType(TokenType.OpenParenthesis)
            ? SyntaxNode.Call(node.Token, GetArguments(context))
            : SyntaxNode.Unspecified(node.Token);

        while (!context.Cursor.IsPeekOneOfType(TokenType.Semicolon, TokenType.Eof))
        {
            switch (current?.Type)
            {
                case TokenType.Dot:
                    current = context.Cursor.Consume();
                    accumulator = SyntaxNode.Get(
                        current,
                        accumulator
                    );
                    context.Cursor.TryConsume(out current);
                    break;
                case TokenType.OpenParenthesis:
                    accumulator = SyntaxNode.Call(current, [.. GetArguments(context), accumulator]);
                    break;
                default:
                    return accumulator;
            }
        }

        return accumulator;
    }

    #endregion
}