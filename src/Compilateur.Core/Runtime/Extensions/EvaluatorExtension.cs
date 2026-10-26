using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime.Extensions;

public static class EvaluatorExtension
{
    extension(Runtime runtime)
    {
        #region Methods

        public object? Negate(ExpressionNode node)
        {
            var value = node.Accept(runtime);
            return value switch
            {
                double d => -d,
                _        => throw runtime.Errors.RuntimeException(node, "Operand must be a number.")
            };
        }

        public object Not(ExpressionNode node)
        {
            var value = node.Accept(runtime);
            return value switch
            {
                null   => true, // null is false. Not false is then true
                bool b => !b,
                _      => false // Default is true in lox. Not true is then false
            };
        }

        #endregion
    }
}