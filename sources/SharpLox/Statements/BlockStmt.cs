using System.Collections.Generic;

namespace SharpLox.Statements;

public sealed class BlockStmt(IEnumerable<IStmt> stmts) : IStmt
{
    public IEnumerable<IStmt> Stmts { get; } = stmts;

    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor) => visitor.Visit(this);
}