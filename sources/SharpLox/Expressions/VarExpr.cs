using SharpLox.Tokens;

namespace SharpLox.Expressions;

public sealed class VarExpr(Token name) : IExpr
{
    public Token Name { get; } = name;

    public TResult Accept<TResult>(IExprVisitor<TResult> visitor) => visitor.Visit(this);
}