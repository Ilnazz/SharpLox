using System.Text;
using SharpLox.Scanning;

namespace SharpLox.Expressions;

/// <summary>
/// The expression stringifier that visualizes an expression like a LISP function application chain.
/// </summary>
public class LispLikeExprStringifier : IExprStringifier, IExprVisitor<string>
{
    public string Stringify(IExpr expr) =>
        expr.Accept(this);

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

    private string Parenthesize(string name, params IExpr[] exprs)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder
            .Append(AsciiChars.LeftParen)
            .Append(name);

        foreach (var expr in exprs)
            stringBuilder
                .Append(AsciiChars.Space)
                .Append(expr.Accept(this));

        stringBuilder
            .Append(AsciiChars.RightParen);

        return stringBuilder.ToString();
    }
}