namespace SharpLox.Statements;

public interface IStmtVisitor<out TResult>
{
    TResult Visit(ExprStmt exprStmt);
    
    TResult Visit(PrintStmt print);
    
    TResult Visit(VarStmt var);

    TResult Visit(BlockStmt block);
}