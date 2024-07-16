namespace SharpLox.Expressions;

public sealed class GroupingExpr(IExpr expr) : IExpr
{
    public IExpr Expr { get; } = expr;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}