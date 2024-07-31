using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using SharpLox.Errors;
using SharpLox.Expressions;
using SharpLox.Scanning;
using SharpLox.Statements;
using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public sealed class Interpreter
(
    IEnvironmentFactory environmentFactory,
    IErrorReporter errorReporter
)
    : IInterpreter, IExprVisitor<object?>, IStmtVisitor<Unit>
{
    private IEnvironment _environment = environmentFactory.CreateEnvironment(null);

    #region Public methods
    public void Interpret(IEnumerable<IStmt> stmts) => WithTryCatch(() =>
    {
        foreach (var stmt in stmts)
            ExecuteStmt(stmt);
    });

    public void Interpret(IExpr expr) => WithTryCatch(() =>
        EvaluateExprAndPrintResult(expr));
    #endregion

    #region Statement visiting methods
    Unit IStmtVisitor<Unit>.Visit(ExprStmt exprStmt)
    {
        EvaluateExpr(exprStmt.Expr);
        return Unit.Default;
    }

    Unit IStmtVisitor<Unit>.Visit(PrintStmt print)
    {
        EvaluateExprAndPrintResult(print.Expr);
        return Unit.Default;
    }

    Unit IStmtVisitor<Unit>.Visit(VarStmt var)
    {
        var varName = var.Name.Lexeme!;
        _environment.Define(varName);
        
        if (var.Initializer is not null)
        {
            var value = EvaluateExpr(var.Initializer);
            _environment.Assign(var.Name, value);
        }
        
        return Unit.Default;
    }

    Unit IStmtVisitor<Unit>.Visit(BlockStmt block)
    {
        var environment = environmentFactory.CreateEnvironment(_environment);
        ExecuteBlock(block.Stmts, environment);
        return Unit.Default;
    }
    
    private void ExecuteStmt(IStmt stmt) => stmt.Accept(this);

    private void ExecuteBlock(IEnumerable<IStmt> stmts, IEnvironment environment)
    {
        var previousEnvironment = _environment;
        
        try
        {
            _environment = environment;
            
            foreach (var stmt in stmts)
                ExecuteStmt(stmt);
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }
    #endregion
    
    #region Expression visiting methods
    object? IExprVisitor<object?>.Visit(LiteralExpr literal) => literal.Value;

    object? IExprVisitor<object?>.Visit(GroupingExpr grouping) => EvaluateExpr(grouping.Expr);

    object? IExprVisitor<object?>.Visit(UnaryExpr unary)
    {
        var right = EvaluateExpr(unary.Right);

        switch (unary.Operator.Type)
        {
            case TokenType.Minus:
            {
                EnsureOperandIsNumber(unary.Operator, right);
                return -(double)right!;
            }
            case TokenType.Bang:
            {
                return !IsTruthy(right);
            }
            default:
            {
                return null;
            }
        }
    }

    object? IExprVisitor<object?>.Visit(BinaryExpr binary)
    {
        var left = EvaluateExpr(binary.Left);
        var right = EvaluateExpr(binary.Right);

        switch (binary.Operator.Type)
        {
            // The comma separated expressions support.
            case TokenType.Comma:
            {
                return right;
            }
            
            case TokenType.Less:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! < (double)right!;
            }
            case TokenType.Greater:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! > (double)right!;
            }
            case TokenType.LessEqual:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! <= (double)right!;
            }
            case TokenType.GreaterEqual:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! >= (double)right!;
            }
            
            case TokenType.BangEqual:
            {
                return !AreEqual(left, right);
            }
            case TokenType.EqualEqual:
            {
                return AreEqual(left, right);
            }

            case TokenType.Slash:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                
                double leftNumber = (double)left!,
                    rightNumber = (double)right!;

                if (rightNumber is 0)
                    throw new RuntimeException(binary.Operator, "It is not possible to divide a number by zero.");
                
                return leftNumber / rightNumber;
            }
            case TokenType.Star:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! * (double)right!;
            }
            case TokenType.Minus:
            {
                EnsureOperandsAreNumbers(binary.Operator, left, right);
                return (double)left! - (double)right!;
            }
            case TokenType.Plus:
            {
                if (left is double leftNumber && right is double rightNumber)
                    return leftNumber + rightNumber;
                
                if (left is string || right is string)
                    return $"{StringifyObject(left)}{StringifyObject(right)}";

                throw new RuntimeException(binary.Operator,
                    "Operands must be two numbers or one of them must be a string.");
            }
            
            default:
            {
                return null;
            }
        }
    }

    object? IExprVisitor<object?>.Visit(ConditionalExpr conditional)
    {
        var conditionResult = EvaluateExpr(conditional.Condition);
        if (conditionResult is not bool conditionResultBool)
            throw new RuntimeException(conditional.Operator,
                "Condition expression must be evaluated to a boolean.");

        return conditionResultBool
            ? EvaluateExpr(conditional.Then)
            : EvaluateExpr(conditional.Else);
    }

    object? IExprVisitor<object?>.Visit(VarExpr var) =>
        _environment.Get(var.Name);

    object? IExprVisitor<object?>.Visit(AssignExpr assign)
    {
        var value = EvaluateExpr(assign.Value);
        _environment.Assign(assign.Name, value);
        return value;
    }
    
    private object? EvaluateExpr(IExpr expr) => expr.Accept(this);
    #endregion

    #region Utility methods
    private void EvaluateExprAndPrintResult(IExpr expr)
    {
        var value = EvaluateExpr(expr);
        var stringifiedValue = StringifyObject(value);
        Console.WriteLine(stringifiedValue); // Todo: make something like IInterpreterOutputWriter
    }
    
    private static string StringifyObject(object? obj) => obj switch
    {
        null => Keywords.Nil,
        double number => number == double.Floor(number) ? $"{number:F0}" : $"{number}",
        string str => $"\"{str}\"",
        bool @bool => @bool.ToString().ToLower(),
        _ => obj.ToString() ?? string.Empty
    };

    private static Error CreateError(Token token, string errorMessage) =>
        new(ErrorType.RuntimeError, token.LineNumber, token.ColumnNumber, errorMessage);
    
    private static bool IsTruthy(object? obj) => obj switch
    {
        bool @bool => @bool,
        null => false,
        _ => true
    };

    private static bool AreEqual(object? a, object? b) => a switch
    {
        null when b is null => true,
        null => false,
        _ => a.Equals(b)
    };

    private static void EnsureOperandsAreNumbers(Token @operator, params object?[] operands)
    {
        if (operands.Any(operand => operand is not double))
            throw new RuntimeException(@operator, "Operands must be numbers.");
    }
    
    private static void EnsureOperandIsNumber(Token @operator, object? operand)
    {
        if (operand is not double)
            throw new RuntimeException(@operator, "Operand must be a number.");
    }
    
    private void WithTryCatch(Action action)
    {
        try
        {
            action();
        }
        catch (RuntimeException e)
        {
            errorReporter.ReportError(CreateError(e.Token, e.Message));
        }
    }
    #endregion
}