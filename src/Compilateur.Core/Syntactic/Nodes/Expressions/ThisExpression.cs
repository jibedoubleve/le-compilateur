using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("this")]
public sealed partial class ThisExpression(Token token) : ExpressionNode(token);