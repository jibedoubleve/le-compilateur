using Compilateur.Core.Syntactic.Rules.Expressions;

namespace Compilateur.Core.Syntactic.Rules;

public interface IParser
{
    #region Methods

    bool Matches(ParsingContext context);

    SyntaxNode? Parse(ParsingContext context);

    #endregion
}