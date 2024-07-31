namespace SharpLox.Statements;

public interface IStmt
{
    TResult Accept<TResult>(IStmtVisitor<TResult> visitor);
}