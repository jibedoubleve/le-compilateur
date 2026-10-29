using Compilateur.Cli.Helpers;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Compilateur.Cli.Commands;

public class ReplCommand : Command
{
    #region Methods

    private static void Title()
    {
        AnsiConsole.Write(new Rule("[blue]REPL[/]")
        {
            Justification = Justify.Left
        });
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        // Get code
        Title();
        var code = string.Empty;
        var textWriter = new StringWriter();
        var compiler = new Compiler(textWriter);
        do
        {
            var prompt = new TextPrompt<string>("[cyan]Your code[/]?");
            code = AnsiConsole.Prompt(prompt);

            if (code == "/quit") { return 0; }

            // Analyse it 
            var analysis = compiler.Analyse(code);

            if (analysis.HasErrors)
            {
                analysis.Errors.Output();
                continue;
            }

            // Run it
            try
            {
                compiler.Run(analysis);

                var output = textWriter.ToString();
                if (!string.IsNullOrEmpty(output))
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(new Rule("[blue]Output[/]")
                    {
                        Justification = Justify.Left
                    });
                    AnsiConsole.WriteLine(output);
                    textWriter.GetStringBuilder().Clear();
                }
            }
            catch (RuntimeException ex)
            {
                ex.Errors.Output();
            }
        } while (true);
    }

    #endregion
}