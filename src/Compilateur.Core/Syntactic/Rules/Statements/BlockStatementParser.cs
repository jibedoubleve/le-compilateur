using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class BlockStatementParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context)
        => context.Cursor.IsPeekOfType(TokenType.OpenCurlyBracket);

    public SyntaxNode? Parse(ParsingContext context) => BlockParser.Parse(context);

    #endregion
}