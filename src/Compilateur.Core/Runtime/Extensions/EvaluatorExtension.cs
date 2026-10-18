using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic.Nodes;

namespace Compilateur.Core.Runtime.Extensions;

public static class EvaluatorExtension
{
    #region Methods

    extension(Evaluator evaluator)
    {
        private object? HandleUnsupportedType(Token token, object? value)
        {
            evaluator.Errors.Add(token, $"Unsupported type '{value?.GetType().Name ?? "null"}'");
            return null;
        }

        public object? Negate(ExpressionNode node)
        {
            var value = node.Accept(evaluator);
            return value switch
            {
                double d => 0 - d,
                _        => evaluator.HandleUnsupportedType(node.Token, value)
            };
        }

        public object Not(ExpressionNode node)
        {
            var value = node.Accept(evaluator);
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