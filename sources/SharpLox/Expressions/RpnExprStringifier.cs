using System.Text;
using SharpLox.Scanning;
using SharpLox.Tokens;

namespace SharpLox.Expressions;

/// <summary>
/// The Reverse Polish Notation expression stringifier.
/// </summary>
public class RpnExprStringifier : IExprStringifier, IExprVisitor<string>
{
    public string Stringify(IExpr expr) =>
        expr.Accept(this);

    string IExprVisitor<string>.Visit(LiteralExpr literal) =>
        literal.Value is not null
            ? literal.Value.ToString() ?? string.Empty
            : "nil";

    string IExprVisitor<string>.Visit(GroupingExpr grouping) =>
        grouping.Expr.Accept(this);

    string IExprVisitor<string>.Visit(UnaryExpr unary) =>
        StringifyExprWithOperator(unary.Operator, unary.Right);

    string IExprVisitor<string>.Visit(BinaryExpr binary) =>
        StringifyExprWithOperator(binary.Operator, binary.Left, binary.Right);

    private string StringifyExprWithOperator(Token @operator, params IExpr[] exprs)
    {
        var stringBuilder = new StringBuilder();

        foreach (var expr in exprs)
            stringBuilder
                .Append(expr.Accept(this))
                .Append(AsciiChars.Space);

        stringBuilder
            .Append(@operator.Lexeme);
        
        return stringBuilder.ToString();
    }
}