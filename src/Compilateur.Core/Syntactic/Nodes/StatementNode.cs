using System.ComponentModel;
using Compilateur.Core.Lexical.Tokens;

namespace Compilateur.Core.Syntactic.Nodes;

[Description("statement")]
public abstract class StatementNode(Token token) : SyntaxNode(token);