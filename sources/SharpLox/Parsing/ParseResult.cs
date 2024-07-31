using System.Collections.Generic;
using SharpLox.Expressions;
using SharpLox.Statements;

namespace SharpLox.Parsing;

public struct ParseResult(ParseResultType type, IExpr? expr = null, IEnumerable<IStmt>? stmts = null)
{
    public ParseResultType Type { get; } = type;

    public IExpr? Expr { get; } = expr;

    public IEnumerable<IStmt>? Stmts { get; } = stmts;
}