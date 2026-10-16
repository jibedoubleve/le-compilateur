using Compilateur.Core.Errors;
using Compilateur.Core.Lexical;
using Compilateur.Core.Runtime;
using Compilateur.Core.Semantic;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Parsers;

namespace Compilateur.Core;

public class Compiler
{
    private readonly TextWriter _output;

    public Compiler(TextWriter output) => _output = output;

    public Compiler() => _output = Console.Out;

    #region Methods

    public  ErrorCollection Compile(string code)
    {
        var nodes = new Scanner().Tokenize(code);
        var cursor = new TokenCursor(nodes.Tokens);
        var context = new ParsingContext(cursor, nodes.Errors);
        var node = ProgramParser.Parse(context);

        var binder = new Binder(context.Errors);

        if (node is null) { return context.Errors;}
        
        binder.Visit(node);

        var interpreter = new Interpreter(binder.Errors, _output);
        return interpreter.Interpret(node);
    }


    #endregion
}