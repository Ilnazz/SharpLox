using System.Text;
using SharpLox.Expressions;
using SharpLox.Scanning;

namespace SharpLox.Utilities;

/// <summary>
/// The Reverse Polish Notation expression stringifier.
/// </summary>
public class RpnExprStringifier : IExprStringifier, IExprVisitor<string>
{
    public string Stringify(IExpr expr) =>
        expr.Accept(this);

    #region Visit methods
    string IExprVisitor<string>.Visit(LiteralExpr literal) =>
        literal.Value is not null
            ? literal.Value.ToString() ?? string.Empty
            : Keywords.Nil;

    string IExprVisitor<string>.Visit(GroupingExpr grouping) =>
        grouping.Expr.Accept(this);

    string IExprVisitor<string>.Visit(UnaryExpr unary) =>
        StringifyExpressions(unary.Operator.Lexeme!, unary.Right);

    string IExprVisitor<string>.Visit(BinaryExpr binary) =>
        StringifyExpressions(binary.Operator.Lexeme!, binary.Left, binary.Right);

    string IExprVisitor<string>.Visit(ConditionalExpr conditional) =>
        StringifyExpressions($"{AsciiChars.Question}{AsciiChars.Colon}",
            conditional.Condition, conditional.Then, conditional.Else);
    #endregion
    
    private string StringifyExpressions(string operatorName, params IExpr[] exprs)
    {
        var stringBuilder = new StringBuilder();

        foreach (var expr in exprs)
            stringBuilder
                .Append(expr.Accept(this))
                .Append(AsciiChars.Space);

        stringBuilder
            .Append(operatorName);
        
        return stringBuilder.ToString();
    }
}