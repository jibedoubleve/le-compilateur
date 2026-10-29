using Compilateur.Cli.Helpers;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Compilateur.Cli.Commands;

public class ExecSettings : CommandSettings
{
    #region Properties

    [CommandArgument(0, "<PATH>")]
    public required string FilePath { get; init; }

    #endregion
}

public class ExecCommand : Command<ExecSettings>
{
    #region Methods

    private static void Title(ExecSettings settings)
    {
        AnsiConsole.Write(new Rule("[blue]Compilation[/]")
        {
            Justification = Justify.Left
        });
        AnsiConsole.Markup("[b]File : [/]");
        var path = new TextPath(settings.FilePath)
                   .RootStyle(new Style(Color.Yellow, decoration: Decoration.Bold))
                   .SeparatorStyle(new Style(Color.Grey))
                   .StemStyle(new Style(Color.Blue))
                   .LeafStyle(new Style(Color.Green, decoration: Decoration.Underline));

        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    public override int Execute(CommandContext context, ExecSettings settings, CancellationToken cancellationToken)
    {
        if (!Path.Exists(settings.FilePath)) { throw new FileNotFoundException(); }

        // Get code
        Title(settings);

        var code = File.ReadAllText(settings.FilePath);

        // Analyse it 
        var compiler = new Compiler();
        var analysis = compiler.Analyse(code);

        if (analysis.HasErrors)
        {
            analysis.Errors.Output();
            return -65;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[blue]Output[/]")
        {
            Justification = Justify.Left
        });

        // Run it
        try
        {
            compiler.Run(analysis);
            return 0;
        }
        catch (RuntimeException ex)
        {
            ex.Errors.Output();
            return -70;
        }
    }

    #endregion
}