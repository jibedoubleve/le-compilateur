using Compilateur.Core.Syntactic;
using Xunit.Abstractions;

namespace Compilateur.Tests.Helpers;

public static class TestOutputHelperExtension
{
    extension(ITestOutputHelper output)
    {
        #region Methods

        public void WriteCode(ParsingContext context)
            => output.WriteLine($"""
                                 Code:
                                 ----- 
                                 {context.Cursor}
                                 """);

        public void WriteSyntaxTree(SyntaxNode? node)
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

        #endregion
    }
}