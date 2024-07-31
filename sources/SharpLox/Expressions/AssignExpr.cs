using SharpLox.Tokens;

namespace SharpLox.Expressions;

public sealed class AssignExpr(Token name, IExpr value) : IExpr
{
    public Token Name { get; } = name;

    public IExpr Value { get; } = value;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}