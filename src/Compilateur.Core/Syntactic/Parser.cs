using Compilateur.Core.Lexical.Tokens;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Syntactic;

public class Parser
{
    #region Fields

    private readonly ILogger _logger;
    private readonly IEnumerable<Token> _tokens;

    #endregion

    #region Constructors

    public Parser(ILogger logger) => _logger = logger;

    #endregion

    #region Methods

    public object? Parse(IReadOnlyCollection<Token> tokens) => null;

    #endregion
}