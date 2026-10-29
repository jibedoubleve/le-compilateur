using Compilateur.Core.Lexical;
using Compilateur.Core.Runtime;
using Compilateur.Core.Semantic;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;

namespace Compilateur.Core;

public class Compiler
{
    #region Fields

    private readonly Interpreter _interpreter;

    #endregion

    #region Constructors

    public Compiler(TextWriter output) => _interpreter = new Interpreter(output);

    public Compiler() : this(Console.Out) { }

    #endregion

    #region Methods

#pragma warning disable S2325
    public Analysis Analyse(string code)
    {
        // Lexical
        var nodes = new Scanner().Tokenize(code);
        var cursor = new TokenCursor(nodes.Tokens);

        // Syntactic
        var context = new ParsingContext(cursor, nodes.Errors);
        var ast = ProgramParser.Parse(context);

        if (ast is null) { return Analysis.Error(context.Errors); }

        // Semantic
        var resolver = new Resolver(context.Errors);
        resolver.Visit(ast);

        // Aggregates syntactic & semantic error
        // and returns if any
        return context.Errors.Any()
            ? Analysis.Error(context.Errors)
            : Analysis.Success(ast, resolver.DistanceMap);
    }
#pragma warning restore S2325

    public void Run(Analysis analysis)
    {
        if (analysis.HasErrors)
        {
            throw new ArgumentException("Invalid analysis", nameof(analysis));
        }

        _interpreter.Interpret(analysis);
    }

    #endregion
}