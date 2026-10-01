using System.Text;
using Compilateur.Core.Extensions;
using Compilateur.Core.Lexical;
using Compilateur.Core.Semantic;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Helpers;
using Compilateur.Core.Syntactic.Nodes;
using Xunit.Abstractions;

namespace Compilateur.Tests.Helpers;

public static class TestOutputHelperExtension
{
    extension(ITestOutputHelper output)
    {
        #region Methods

        private void WriteCode(ParsingContext context)
            => output.WriteLine($"""
                                 Code:
                                 ----- 
                                 {context.Cursor}
                                 """);

        private void WriteSyntaxTree(SyntaxNode? node)
        {
            if (node is null)
            {
                output.WriteLine("Syntax tree: EMPTY.");
                return;
            }

            output.WriteLine("Syntax tree:");
            output.WriteLine("------------");
            output.WriteLine(
                node.FormatTree()
            );
        }

        public void WriteLexerContext(string code, TokenizeResult tokenization)
        {
            output.WriteLine($"""
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

            output.WriteLine("""
                             | Lexeme | Kind               |
                             | ------ | ------------------ |
                             """);
            foreach (var token in tokenization.Tokens)
            {
                output.WriteLine($"| {token.Lexeme,6} | {token.Kind,-18} |");
            }
        }

        public void WriteSemanticContext(Binder binder)
        {
            var sb = new StringBuilder();
            sb.AppendLine("""

                          Distance map:
                          -------------
                          |          Variable          | Distance |
                          | -------------------------- | -------- |

                          """);
            foreach (var local in binder.ToLocals())
            {
                sb.AppendLine($"| {local.Name,26} | {local.Distance,8} |");
            }

            sb.AppendLine("""
                          Errors:
                          ------
                          """);
            sb.AppendLine(binder.Errors.Format());

            output.WriteLine(sb.ToString());
        }

        public void WriteSyntaxContext(ParsingContext context, SyntaxNode? node = null)
        {
            output.WriteCode(context);
            if (node != null) { output.WriteSyntaxTree(node); }
            else { output.WriteLine("No syntax tree to output."); }

            output.WriteLine(context.FormatErrors());
        }

        #endregion
    }
}

public record Local(string Name, uint Distance);

public static class DistanceMapExtensions
{
    #region Methods

    public static IEnumerable<Local> ToLocals(this Binder binder)
        => binder.DistanceMap.Select(x => new Local(
            $"{x.Key.Token.Lexeme} ({x.Key.Token.Line},{x.Key.Token.Column})",
            x.Value)
        );

    #endregion
}