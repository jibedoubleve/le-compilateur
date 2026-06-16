namespace Compilateur.Core.Syntactic.Rules.Expressions;

internal class TermParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context)
    {
        if (context.Cursor.IsAtEnd) { return false; }

        var current = context.Cursor.Peek();
        var next = context.Cursor.PeekNext();

        if (next is null)
        {
            return false;
        }

        return current.IsNumeric() && next.IsTerm();
    }

    public SyntaxNode? Parse(ParsingContext context)
    {
        var cursor = context.Cursor;
        var left = cursor.Consume();
        var operation = cursor.Consume();

        if (cursor.IsAtEnd)
        {
            context.AddError(operation, "Unexpected end if file");
            return null;
        }

        var right = cursor.Consume();
        
        if (cursor.IsAtEnd)
        {
            context.AddError(right, "Unexpected end if file");
            return null;
        }

        if (!left.IsNumeric())
        {
            context.AddError(left, $"The left part of a '{left.Lexeme}' has to be numeric");
            return null;
        }

        var rightNext = cursor.PeekNext();
        if (right.IsNumeric() && rightNext.IsTerm()) { return context.BuildTerm(); }

        if (right.IsNumeric() && rightNext.IsFactor()) { return context.BuildFactor(); }

        if (right.IsNumeric()) { return new SyntaxNode(operation); }

        context.AddError(left, "Incomplete operation");

        return null;
    }

    #endregion
}