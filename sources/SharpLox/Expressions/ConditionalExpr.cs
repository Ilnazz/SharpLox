namespace SharpLox.Expressions;

public sealed class ConditionalExpr(IExpr condition, IExpr then, IExpr @else) : IExpr
{
    public IExpr Condition { get; } = condition;

    public IExpr Then { get; } = then;

    public IExpr Else { get; } = @else;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}