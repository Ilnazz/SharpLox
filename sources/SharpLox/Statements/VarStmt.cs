using SharpLox.Expressions;
using SharpLox.Tokens;

namespace SharpLox.Statements;

public sealed class VarStmt(Token name, IExpr? initializer) : IStmt
{
    public Token Name { get; } = name;

    public IExpr? Initializer { get; } = initializer;

    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor) => visitor.Visit(this);
}