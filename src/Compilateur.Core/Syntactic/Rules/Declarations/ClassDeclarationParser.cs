using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;

namespace Compilateur.Core.Syntactic.Rules.Declarations;

internal class ClassDeclarationParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context) => context.Cursor.IsPeekOfType(TokenType.Class);

    public SyntaxNode? Parse(ParsingContext context)
    {
        context.Cursor.Consume(); // consume the 'class'
        var className = context.Cursor.Peek();
        if (className.Type != TokenType.Identifier)
        {
            context.AddError("Expected an identifier after 'class'");
            return null;
        }

        context.Cursor.Consume(); // Consume the identifier

        
        // Handle the functions & heritage
        var children = new List<SyntaxNode>();
        
        if (context.Cursor.Peek().Type == TokenType.LessThan)
        {
            context.Cursor.Consume(); // Consume '<'
            var current = context.Cursor.Peek();
            if (current.Type != TokenType.Identifier)
            {
                context.AddError("Expected an identifier after '<'");
                return null;
            }
            
            children.Add(SyntaxNode.Class(current));
            context.Cursor.Consume(); // consume the identifier
        }

        if (!context.Cursor.IsPeekOfType(TokenType.OpenCurlyBracket))
        {
            context.AddError($"Expected an opening bracket after 'class {className.Lexeme}'");
            return null;
        }

        context.Cursor.Consume(); // Consume the '{'

        while (true)
        {
            if (context.Cursor.IsPeekOfType(TokenType.CloseCurlyBracket))
            {
                break;
            }

            if (context.Cursor.IsAtEnd)
            {
                context.AddError("No closing bracket after definition of class");
                return null;
            }

            var node = FunctionParser.Parse(context);

            if (node is null) { return null; }

            children.Add(node);
        }

        return SyntaxNode.Class(className, [.. children]);
    }

    #endregion
}