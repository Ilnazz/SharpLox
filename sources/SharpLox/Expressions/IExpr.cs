namespace SharpLox.Expressions;

public interface IExpr
{
    TResult Accept<TResult>(IExprVisitor<TResult> visitor);
}