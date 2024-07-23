using SharpLox.Tokens;

namespace SharpLox.Expressions;

// Todo: store '?' or ':' or both as operators?
public sealed class ConditionalExpr
(
    Token @operator,
    IExpr condition,
    IExpr then,
    IExpr @else
)
    : IExpr
{
    public Token Operator { get; } = @operator;

    public IExpr Condition { get; } = condition;

    public IExpr Then { get; } = then;

    public IExpr Else { get; } = @else;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}