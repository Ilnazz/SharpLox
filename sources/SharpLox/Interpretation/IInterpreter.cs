using System.Collections.Generic;
using SharpLox.Expressions;
using SharpLox.Statements;

namespace SharpLox.Interpretation;

public interface IInterpreter
{
    void Interpret(IExpr expr);

    void Interpret(IEnumerable<IStmt> stmts);
}