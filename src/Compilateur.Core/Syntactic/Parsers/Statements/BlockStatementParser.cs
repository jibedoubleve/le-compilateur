using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic.Parsers.Statements;

public class BlockStatementParser : Parser<StatementNode>
{
    #region Methods

    public override bool Matches(ParsingContext context)
        => context.Cursor.IsPeekOfKind(TokenKind.OpenCurlyBracket);

    protected override StatementNode? ParseCore(ParsingContext context) => BlockParser.Parse(context);

    #endregion
}