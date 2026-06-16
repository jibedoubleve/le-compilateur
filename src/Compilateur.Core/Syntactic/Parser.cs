using Compilateur.Core.Errors;
using Compilateur.Core.Syntactic.Rules.Declarations;
using Compilateur.Core.Syntactic.Rules.Expressions;
using Microsoft.Extensions.Logging;

namespace Compilateur.Core.Syntactic;

public class Parser
{
    #region Fields

    private readonly ILogger _logger;

    #endregion

    #region Constructors

    public Parser(ILogger logger) => _logger = logger;

    #endregion

    #region Methods

    public SyntaxNode Parse(ParsingContext context)
    {
        var errors = new SyntaxErrorCollection();
        var nodes = new List<SyntaxNode>();

        while (!context.Cursor.IsAtEnd)
        {
            var node = new DeclarationParser().Parse(context);
            if (node is not null)
            {
                nodes.Add(node);
            }
        }

        return new SyntaxNode(context.Cursor.Peek(), nodes.ToArray());
    }

    #endregion
}