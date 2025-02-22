﻿namespace SharpLox.Expressions;

public interface IExprVisitor<out TResult>
{
    TResult Visit(LiteralExpr literal);
    
    TResult Visit(GroupingExpr grouping);

    TResult Visit(UnaryExpr unary);

    TResult Visit(BinaryExpr binary);

    TResult Visit(ConditionalExpr conditional);

    TResult Visit(VarExpr var);

    TResult Visit(AssignExpr assign);
}