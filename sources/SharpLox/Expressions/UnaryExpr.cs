using SharpLox.Tokens;

namespace SharpLox.Expressions;

public sealed class UnaryExpr(Token @operator, IExpr right) : IExpr
{
    public Token Operator { get; } = @operator;

    public IExpr Right { get; } = right;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}