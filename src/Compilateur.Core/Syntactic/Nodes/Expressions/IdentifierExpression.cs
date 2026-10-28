using System.ComponentModel;
using System.Diagnostics;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes.Expressions;

[Description("identifier")]
[DebuggerDisplay("{Token.Lexeme}")]
public sealed partial class IdentifierExpression(Token token) : ExpressionNode(token);