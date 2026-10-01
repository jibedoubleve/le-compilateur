using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

public abstract class ExpressionNode(Token token) : SyntaxNode(token);