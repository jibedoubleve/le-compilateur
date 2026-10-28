using System.Text;
using Compilateur.Core;
using Compilateur.Core.Errors;
using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical;
using Compilateur.Core.Semantic;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Tests.Helpers;

public static class TestOutputHelperExtension
{
    #region Methods

    private static void WriteCode(this ITestOutputHelper outputHelper, ParsingContext context)
        => outputHelper.WriteLine($"""
                                   Code:
                                   ----- 
                                   {context.Cursor}
                                   """);

    private static void WriteSyntaxTree(this ITestOutputHelper outputHelper, SyntaxNode? node)
    {
        if (node is null)
        {
            outputHelper.WriteLine("Syntax tree: EMPTY.");
            return;
        }

        outputHelper.WriteLine("Syntax tree:");
        outputHelper.WriteLine("------------");
        outputHelper.WriteLine(
            node.FormatTree()
        );
    }

    public static void WriteAnalysisContext(this ITestOutputHelper outputHelper, Analysis analysis) =>
        outputHelper.WriteErrors(analysis.Errors);

    public static void WriteErrors(this ITestOutputHelper outputHelper, ErrorCollection errors) =>
        outputHelper.WriteLine($"""

                                Errors:
                                -------

                                {errors.Format()}
                                """);

    public static void WriteLexerContext(this ITestOutputHelper outputHelper, string code, TokenizeResult tokenization)
    {
        outputHelper.WriteLine($"""
                                Code:
                                -----

                                {code}

                                Errors:
                                -------

                                {tokenization.Errors.Format()}

                                Tokens:
                                -------
                                """
        );

        outputHelper.WriteLine("""
                               | Lexeme | Kind               |
                               | ------ | ------------------ |
                               """);
        foreach (var token in tokenization.Tokens)
        {
            outputHelper.WriteLine($"| {token.Lexeme,6} | {token.Kind,-18} |");
        }
    }

    public static void WriteSemanticContext(this ITestOutputHelper outputHelper, Resolver resolver)
    {
        var sb = new StringBuilder();
        sb.AppendLine("""

                      Distance map:
                      -------------
                      |          Variable          | Distance |
                      | -------------------------- | -------- |

                      """);
        foreach (var local in resolver.ToLocals())
        {
            sb.AppendLine($"| {local.Name,26} | {local.Distance,8} |");
        }

        sb.AppendLine("""
                      Errors:
                      ------
                      """);
        sb.AppendLine(resolver.Errors.Format());

        outputHelper.WriteLine(sb.ToString());
    }

    public static void WriteSyntaxContext(
        this ITestOutputHelper outputHelper, ParsingContext context, SyntaxNode? node = null)
    {
        outputHelper.WriteCode(context);
        if (node != null) { outputHelper.WriteSyntaxTree(node); }
        else { outputHelper.WriteLine("No syntax tree to output."); }

        outputHelper.WriteLine(context.FormatErrors());
    }

    #endregion
}

public record Local(string Name, uint Distance);

public static class DistanceMapExtensions
{
    #region Methods

    public static IEnumerable<Local> ToLocals(this Resolver resolver)
        => resolver.DistanceMap.Select(x => new Local(
            $"{x.Key.Token.Lexeme} ({x.Key.Token.Line},{x.Key.Token.Column})",
            x.Value)
        );

    #endregion
}