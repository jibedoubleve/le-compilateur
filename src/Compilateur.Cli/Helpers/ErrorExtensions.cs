using Compilateur.Core;
using Compilateur.Core.Errors;
using Spectre.Console;

namespace Compilateur.Cli.Helpers;

public static class AnalysisExtensions
{
    #region Methods

    public static void Output(this ErrorCollection errors)
    {
        var table = new Table();
        table.RoundedBorder();
        
        table.AddColumn("Line");
        table.AddColumn("Col");
        table.AddColumn("Token");
        table.AddColumn("Message");

        foreach (var error in errors)
        {
            table.AddRow($"{error.Line}", $"{error.Column}", error.Variable,  error.Message);
        }
     
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[red]Errors[/]")
        {
            Justification = Justify.Left
        });
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
    }

    #endregion
}