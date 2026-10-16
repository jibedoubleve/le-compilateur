using System.Reflection;
using Compilateur.Core.Lexical.Tokens;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Nodes;
using Compilateur.Core.Syntactic.Nodes.Declaration;
using Compilateur.Core.Syntactic.Nodes.Expressions;
using Compilateur.Core.Syntactic.Nodes.Statements;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Binder = Compilateur.Core.Semantic.Binder;

namespace Compilateur.Tests;

public class TextSyntacNodeVisitor : ISyntaxNodeVisitor<int>
{
    #region Fields

    private readonly int _value;

    #endregion

    #region Constructors

    public TextSyntacNodeVisitor(int value) => _value = value;

    #endregion

    #region Methods

    public int Visit(ClassStatement node) => _value;

    public int Visit(FunctionDeclarationStatement node) => _value;

    public int Visit(VarDeclarationStatement node) => _value;

    public int Visit(AssignExpression node) => _value;

    public int Visit(BinaryExpression node) => _value;

    public int Visit(CallExpression node) => _value;

    public int Visit(GetExpression node) => _value;

    public int Visit(GroupExpressionNode node) => _value;

    public int Visit(IdentifierExpression node) => _value;

    public int Visit(LiteralExpression node) => _value;

    public int Visit(LogicalExpression node) => _value;

    public int Visit(SetExpression node) => _value;

    public int Visit(SuperExpression node) => _value;

    public int Visit(ThisExpression node) => _value;

    public int Visit(UnaryExpression node) => _value;

    public int Visit(BlockStatement node) => _value;

    public int Visit(ExpressionStatement node) => _value;

    public int Visit(IfStatement node) => _value;

    public int Visit(PrintStatement node) => _value;

    public int Visit(ReturnStatement node) => _value;

    public int Visit(WhileStatement node) => _value;

    public int Visit(ProgramNode node) => 42;

    #endregion
}

public class SyntaxNodeVisitorTest
{
    #region Methods

    [Fact]
    public void When_Accept_Then_Returns_Visitor_Result()
    {
        // arrange
        const int val = 42;
        var visitor = new TextSyntacNodeVisitor(val);

        var token = new Token
        {
            Column = 0,
            Line = 0,
            Kind = TokenKind.And,
            Lexeme = "",
            Value = 44
        };
        var id = new IdentifierExpression(token);

        var program = new ProgramNode(token, []);

        // act & assert
        program.Accept(visitor).ShouldBe(val);
        

    }

    #endregion
}