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

    #region Visiting methods
    string IExprVisitor<string>.Visit(LiteralExpr literal) =>
        literal.Value is not null
            ? literal.Value.ToString() ?? string.Empty
            : Keywords.Nil;

    string IExprVisitor<string>.Visit(GroupingExpr grouping) =>
        Parenthesize("group", grouping.Expr);

    string IExprVisitor<string>.Visit(UnaryExpr unary) =>
        Parenthesize(unary.Operator.Lexeme!, unary.Right);

    string IExprVisitor<string>.Visit(BinaryExpr binary) =>
        Parenthesize(binary.Operator.Lexeme!, binary.Left, binary.Right);

    string IExprVisitor<string>.Visit(ConditionalExpr conditional) =>
        Parenthesize($"{AsciiChars.Question}{AsciiChars.Colon}",
            conditional.Condition, conditional.Then, conditional.Else);

    string IExprVisitor<string>.Visit(VarExpr var) =>
        var.Name.Lexeme!;

    string IExprVisitor<string>.Visit(AssignExpr assign) =>
        $"{AsciiChars.LeftParen}{AsciiChars.Equal} {assign.Name} {assign.Value}{AsciiChars.RightParen}";
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