using System.Text;
using SharpLox.Expressions;
using SharpLox.Scanning;

namespace SharpLox.Utilities;

/// <summary>
/// The expression stringifier that visualizes an expression like a LISP function application chain.
/// </summary>
public class LispLikeExprStringifier : IExprStringifier, IExprVisitor<string>
{
    public string Stringify(IExpr expr) =>
        expr.Accept(this);

    #region Visit methods
    string IExprVisitor<string>.Visit(LiteralExpr literal) =>
        literal.Value is not null
            ? literal.Value.ToString() ?? string.Empty
            : "nil";

    string IExprVisitor<string>.Visit(GroupingExpr grouping) =>
        Parenthesize("group", grouping.Expr);

    string IExprVisitor<string>.Visit(UnaryExpr unary) =>
        Parenthesize(unary.Operator.Lexeme!, unary.Right);

    string IExprVisitor<string>.Visit(BinaryExpr binary) =>
        Parenthesize(binary.Operator.Lexeme!, binary.Left, binary.Right);

    string IExprVisitor<string>.Visit(ConditionalExpr conditional) =>
        Parenthesize($"{AsciiChars.Question}{AsciiChars.Colon}",
            conditional.Condition, conditional.Then, conditional.Else);
    #endregion

    private string Parenthesize(string operatorName, params IExpr[] exprs)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder
            .Append(AsciiChars.LeftParen)
            .Append(operatorName);

        foreach (var expr in exprs)
            stringBuilder
                .Append(AsciiChars.Space)
                .Append(expr.Accept(this));

        stringBuilder
            .Append(AsciiChars.RightParen);

        return stringBuilder.ToString();
    }
}