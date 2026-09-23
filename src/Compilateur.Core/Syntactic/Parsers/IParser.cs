using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Syntactic.Parsers;

public interface IParser<out T>
    where T : SyntaxNode
{
    #region Methods

    bool Matches(ParsingContext context);

    T? Parse(ParsingContext context);

    #endregion
}