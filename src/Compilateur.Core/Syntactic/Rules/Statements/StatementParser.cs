using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules.Statements;

public class StatementParser : IParser
{
    #region Methods

    public bool Matches(ParsingContext context) => throw new NotImplementedException();

    public SyntaxNode? Parse(ParsingContext context) => throw new NotImplementedException();

    #endregion
}