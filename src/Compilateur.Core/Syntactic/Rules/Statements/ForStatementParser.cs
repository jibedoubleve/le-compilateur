using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Rules.Declarations;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class ForStatementParser : IParser
{
    #region Fields

    private static readonly ExpressionParser ExpressionParser = new();
    private static readonly StatementParser StatementParser = new();
    private static readonly VarDeclarationParser VarDeclarationParser = new();

    #endregion

    #region Methods

    private static bool TryParseInit(ParsingContext context, out SyntaxNode? init)
    {
        if (VarDeclarationParser.Matches(context))
        {
            init = VarDeclarationParser.Parse(context);
            return true;
        }

        init = context.ValidateFinalSemicolon(
            ExpressionParser.Parse(context)
        );
        return init is not null;
    }

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.For);

    public SyntaxNode? Parse(ParsingContext context)
    {
        if (!Matches(context))
        {
            context.AddError($"Expected 'for' but specified '{context.Cursor.Peek().Type}'.");
            return null;
        }

        var keyword = SyntaxNode.Unspecified(context.Cursor.Consume());

        if (!context.Cursor.IsPeekOfType(TokenType.OpenParenthesis))
        {
            context.AddError($"Expected '(' but found '{context.Cursor.Peek().Type}'.");
            return null;
        }

        context.Cursor.Consume(); // Consume the '('

        var children = new List<SyntaxNode>();

        if (!TryParseInit(context, out var init)) { return null; }

        if (init is not null) { children.Add(SyntaxNode.Init(init)); }


        var condition = ExpressionParser.Parse(context);
        if (condition is not null) { children.Add(SyntaxNode.Condition(condition)); }

        if (context.ValidateFinalSemicolon(condition) is null) { return null; }

        var increment = ExpressionParser.Parse(context);
        if (increment is not null) { children.Add(SyntaxNode.Increment(increment)); }

        context.Cursor.Consume(); // Consume the ')'

        var block = StatementParser.Parse(context);
        if (block is null) { return null; }

        children.Add(SyntaxNode.Body(block));

        return SyntaxNode.Unspecified(keyword.Token, [.. children]);
    }

    #endregion
}