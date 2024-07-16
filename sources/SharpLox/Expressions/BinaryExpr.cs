using SharpLox.Tokens;

namespace SharpLox.Expressions;

public sealed class BinaryExpr(IExpr left, Token @operator, IExpr right) : IExpr
{
    public IExpr Left { get; } = left;

    public Token Operator { get; } = @operator;

    public IExpr Right { get; } = right;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}