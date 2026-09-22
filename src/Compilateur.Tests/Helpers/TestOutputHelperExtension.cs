using Compilateur.Core.Extensions;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Helpers;
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

        public void WriteFullContext(ParsingContext context, SyntaxNode? node = null)
        {
            output.WriteCode(context)
                ;
            if (node != null) { output.WriteSyntaxTree(node); }
            else { output.WriteLine("No syntax tree to output."); }

            output.WriteLine(context.FormatErrors());
        }

        #endregion
    }
}