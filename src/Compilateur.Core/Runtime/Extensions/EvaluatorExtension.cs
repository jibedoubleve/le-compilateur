using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime.Extensions;

public static class EvaluatorExtension
{
    #region Methods

    extension(Runtime runtime)
    {
        private object? HandleUnsupportedType(Token token, object? value)
        {
            runtime.Errors.Add(token, $"Unsupported type '{value?.GetType().Name ?? "null"}'");
            return null;
        }

        public object? Negate(ExpressionNode node)
        {
            var value = node.Accept(runtime);
            return value switch
            {
                double d => -d,
                _        => runtime.HandleUnsupportedType(node.Token, value)
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
    }

    #endregion
}