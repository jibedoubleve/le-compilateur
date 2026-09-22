using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
using Compilateur.Core.Syntactic.Rules.Statements;
using Compilateur.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Compilateur.Tests.Syntactic.Statements;

public class WhileStatementTest
{
    #region Fields

    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public WhileStatementTest(ITestOutputHelper output) => _output = output;

    #endregion

    #region Methods

    public static IEnumerable<object[]> BuildInvalidWhile()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.Number(1))
                .OpenCurlyBracket()
                .Var("a")
                .Equal()
                .Number(4)
                .Semicolon()
                .BuildParsingContext()
        ];
        
        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.Number(1))
                .Identifier("a")
                .Equal()
                .Number(4)
                .Semicolon()
                .CloseCurlyBracket()
                .BuildParsingContext()
        ];

        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.Number(1))
                .Var("a")
                .Equal()
                .Number(4)
                .Semicolon()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .OpenParenthesis()
                .CloseParenthesis()
                .BetweenCurlyBracket(b =>
                    b.Identifier("foo")
                     .Equal()
                     .Number(4)
                     .Semicolon())
                .BuildParsingContext()
        ];
    }

    public static IEnumerable<object[]> BuildValidWhile()
    {
        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.Number(1))
                .Identifier("a")
                .Equal()
                .Number(4)
                .Semicolon()
                .BuildParsingContext()
        ];
        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b =>
                    b.Identifier("a")
                     .GreaterThan()
                     .Identifier("b"))
                .BetweenCurlyBracket(c =>
                    c.Identifier("c")
                     .Equal()
                     .Identifier("c")
                     .Plus()
                     .Number(4)
                     .Semicolon())
                .BuildParsingContext()
        ];

        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.True())
                .BetweenCurlyBracket(b =>
                    b.Var("a")
                     .Equal()
                     .Number(4)
                     .Semicolon())
                .BuildParsingContext()
        ];

        yield return
        [
            new TokenCollectionBuilder()
                .While()
                .BetweenParentheses(b => b.Number(1))
                .BetweenCurlyBracket(b =>
                    b.Number(3)
                     .Plus()
                     .Number(4)
                     .Semicolon())
                .BuildParsingContext()
        ];
    }

    [Theory]
    [MemberData(nameof(BuildInvalidWhile))]
    public void When_Invalid_While_Then_Error_Raised(ParsingContext context)
    {
        // act
        var node = ProgramParser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldBeNull(),
            () => context.Errors.ShouldNotBeEmpty()
        );
    }

    [Theory]
    [MemberData(nameof(BuildValidWhile))]
    public void When_Valid_While_Then_Node_Returned(ParsingContext context)
    {
        // arrange
        var parser = new StatementParser();

        // act
        var node = parser.Parse(context);
        _output.WriteFullContext(context, node);

        // assert
        Assert.Multiple(
            () => node.ShouldNotBeNull(),
            () => context.Errors.ShouldBeEmpty()
        );
    }

    #endregion
}