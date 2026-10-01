using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

public sealed partial class LiteralExpression(Token token) : ExpressionNode(token);