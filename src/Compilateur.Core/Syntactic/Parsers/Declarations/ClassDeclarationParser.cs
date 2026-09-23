using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;

namespace Compilateur.Core.Syntactic.Parsers.Declarations;

internal class ClassDeclarationParser : Parser<ClassStatement>
{
    #region Methods

    protected override ClassStatement? ParseCore(ParsingContext context)
    {
        context.Cursor.Consume(); // consume the 'class'
        var className = context.Cursor.Peek();
        if (className.Kind != TokenKind.Identifier)
        {
            context.AddError("Expected an identifier after 'class'");
            return null;
        }

        context.Cursor.Consume(); // Consume the identifier


        // Handle the functions & heritage
        IdentifierExpression? superClass = null;
        if (context.Cursor.Peek().Kind == TokenKind.LessThan)
        {
            context.Cursor.Consume(); // Consume '<'
            var current = context.Cursor.Peek();
            if (current.Kind != TokenKind.Identifier)
            {
                context.AddError("Expected an identifier after '<'");
                return null;
            }

            superClass = new IdentifierExpression(current);
            context.Cursor.Consume(); // consume the identifier
        }

        if (!context.Cursor.IsPeekOfKind(TokenKind.OpenCurlyBracket))
        {
            context.AddError($"Expected an opening bracket after 'class', found {className.Lexeme}'");
            return null;
        }

        context.Cursor.Consume(); // Consume the '{'

        var children = new List<FunctionDeclarationStatement>();
        while (true)
        {
            if (context.Cursor.IsPeekOfKind(TokenKind.CloseCurlyBracket))
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

        return new ClassStatement(className, superClass, [.. children]);
    }

    public override bool Matches(ParsingContext context) => context.Cursor.IsPeekOfKind(TokenKind.Class);

    #endregion
}