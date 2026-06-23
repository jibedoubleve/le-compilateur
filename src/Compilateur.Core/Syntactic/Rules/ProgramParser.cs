namespace Compilateur.Core.Syntactic.Rules;

public class ProgramParser : IParser
{
    #region Fields

    private static readonly DeclarationParser DeclarationParser = new();

    #endregion

    #region Methods

    public bool Matches(ParsingContext context) => throw new NotImplementedException();

    public SyntaxNode? Parse(ParsingContext context)
    {
        var node = DeclarationParser.Parse(context);
        if (node is not null && context.Cursor.IsAtEnd) { return node; }

        context.AddError($"Expected to reach the end of the code but found {context.Cursor.Peek().Lexeme}.");
        return null;
    }

    #endregion
}