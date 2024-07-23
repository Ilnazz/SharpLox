using System;
using System.Linq;
using SharpLox.Errors;
using SharpLox.Expressions;
using SharpLox.Scanning;
using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public sealed class Interpreter(IErrorReporter errorReporter) : IInterpreter, IExprVisitor<object?>
{
    public void Interpret(IExpr expr)
    {
        try
        {
            var value = Evaluate(expr);
            var stringifiedValue = StringifyObject(value);
            Console.WriteLine(stringifiedValue); // Todo: make something like IInterpreterOutput
        }
        catch (RuntimeException e)
        {
            errorReporter.ReportError(CreateError(e.Token, e.Message));
        }
    }
    
    #region Visit methods
    object? IExprVisitor<object?>.Visit(LiteralExpr literal) => literal.Value;

    object? IExprVisitor<object?>.Visit(GroupingExpr grouping) => Evaluate(grouping.Expr);

    object? IExprVisitor<object?>.Visit(UnaryExpr unary)
    {
        var right = Evaluate(unary.Right);

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
        var left = Evaluate(binary.Left);
        var right = Evaluate(binary.Right);

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
                    throw new ZeroDivisionException(binary.Operator, "It is not possible to divide a number by zero.");
                
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
        var conditionResult = Evaluate(conditional.Condition);
        if (conditionResult is not bool conditionResultBool)
            throw new RuntimeException(conditional.Operator,
                "Condition expression must be evaluated to a boolean.");

        return conditionResultBool
            ? Evaluate(conditional.Then)
            : Evaluate(conditional.Else);
    }
    #endregion
    
    private object? Evaluate(IExpr expr) => expr.Accept(this);

    #region Utility methods
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
    #endregion
}