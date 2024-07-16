namespace SharpLox.Expressions;

public sealed class LiteralExpr(object? value) : IExpr
{
    public object? Value { get; } = value;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}