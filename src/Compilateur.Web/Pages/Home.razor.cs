using BlazorMonaco.Editor;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Compilateur.Core.Extensions;
using Compilateur.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Compilateur.Web.Pages;

public partial class Home
{
    #region Fields

    private static readonly string[] Examples =
    [
        .. typeof(Home).Assembly
                       .GetManifestResourceNames()
                       .Where(name => name.EndsWith(".lox"))
                       .Order()
    ];

    private StandaloneCodeEditor _editor = null!;

    private string _fileName = "no file";
    private string _output = "";

    #endregion

    #region Properties

    [Inject]
    public JSRuntime JSRuntime { get; set; } = null!;

    #endregion

    #region Methods

    private async Task ApplyTheme()
    {
        await JSRuntime.InvokeVoidAsync("registerLox");
        await JSRuntime.InvokeVoidAsync("applyTheme");
    }

    private StandaloneEditorConstructionOptions EditorOptions(StandaloneCodeEditor editor) =>
        new() { AutomaticLayout = true };


    private async Task LoadExample(string name)
    {
        await using var stream = typeof(Home).Assembly.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        await _editor.SetValue(await reader.ReadToEndAsync());
        _fileName = FileHelpers.GetFileName(name);
    }

    private async Task NewFile()
    {
        await _editor.SetValue("");
        _fileName = "no file";
        _output = "";
    }

    private async Task Run()
    {
        var writer = new StringWriter();
        var compiler = new Compiler(writer);
        var analysis = compiler.Analyse(await _editor.GetValue());

        if (analysis.HasErrors)
        {
            _output = analysis.Errors.Format();
            return;
        }

        try
        {
            compiler.Run(analysis);
            _output = writer.ToString();
        }
        catch (RuntimeException ex) { _output = writer + ex.Errors.Format(); }
    }

    #endregion
}