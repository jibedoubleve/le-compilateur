using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("identifier")]
public sealed partial class IdentifierExpression(Token token) : ExpressionNode(token);