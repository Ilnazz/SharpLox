using SharpLox.Expressions;

namespace SharpLox.Statements;

public sealed class PrintStmt(IExpr expr) : IStmt
{
    public IExpr Expr { get; } = expr;

    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor) => visitor.Visit(this);
}