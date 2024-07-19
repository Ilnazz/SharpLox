using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpLox.Base;
using SharpLox.Errors;
using SharpLox.Expressions;
using SharpLox.Scanning;
using SharpLox.Tokens;

namespace SharpLox.Parsing;

public sealed class Parser(IErrorReporter errorReporter, IReadOnlyList<Token> tokens) :
    ScannerParserBase<Token>(errorReporter, tokens), IParser
{
    public IExpr? Parse()
    {
        try
        {
            return ParseCommaExpression();
        }
        catch (ParseException)
        {
            return null;
        }
    }

    private IExpr ParseCommaExpression() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseExpression, TokenType.Comma);

    private IExpr ParseExpression() => ParseConditional();

    private IExpr ParseConditional()
    {
        var expr = ParseEquality();

        if (!MatchAndAdvance(TokenType.Question))
            return expr;
        
        var then = ParseEquality();
        Consume(TokenType.Colon, $"expected \"{AsciiChars.Colon}\" in conditional expression");
        var @else = ParseEquality();
        return new ConditionalExpr(expr, then, @else);
    }

    private IExpr ParseEquality() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseComparison, TokenType.BangEqual, TokenType.EqualEqual);

    private IExpr ParseComparison() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseTerm,
            TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual);

    private IExpr ParseTerm() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseFactor, TokenType.Plus, TokenType.Minus);

    private IExpr ParseFactor() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseUnary, TokenType.Slash, TokenType.Star);

    private IExpr ParseUnary()
    {
        if (!MatchAnyAndAdvance(TokenType.Bang, TokenType.Minus))
            return ParsePrimary();
        
        var @operator = Previous;
        var right = ParseUnary();
        return new UnaryExpr(@operator, right);
    }

    private IExpr ParsePrimary()
    {
        if (MatchAndAdvance(TokenType.False))
            return new LiteralExpr(false);
        if (MatchAnyAndAdvance(TokenType.True))
            return new LiteralExpr(true);
        if (MatchAnyAndAdvance(TokenType.Nil))
            return new LiteralExpr(null);

        if (MatchAnyAndAdvance(TokenType.Number, TokenType.String))
            return new LiteralExpr(Previous.Literal);

        if (MatchAnyAndAdvance(TokenType.LeftParen))
        {
            var expr = ParseExpression();
            Consume(TokenType.RightParen, $"expected \"{AsciiChars.RightParen}\" after expression");
            return new GroupingExpr(expr);
        }
        
        // ReportErrorAndThrow("expression expected.");
        ReportError(CreateError(Current, "expression expected"));
        throw new ParseException();
    }

    private void Consume(TokenType tokenType, string errorMessage)
    {
        if (MatchAndAdvance(tokenType))
            return;

        ReportErrorAndThrow(errorMessage);
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd)
        {
            if (Previous.Type is TokenType.Semicolon)
                return;

            switch (Current.Type)
            {
                case TokenType.Class:
                case TokenType.Var:
                case TokenType.Fun:
                case TokenType.Return:
                case TokenType.For:
                case TokenType.While:
                case TokenType.If:
                case TokenType.Print:
                {
                    return;
                }
            }
            
            Advance();
        }
    }

    [DoesNotReturn]
    private void ReportErrorAndThrow(string errorMessage)
    {
        ReportError(CreateError(Current, errorMessage));
        throw new ParseException();
    }

    private static Error CreateError(Token token, string errorMessage)
    {
        errorMessage = token.Type is TokenType.Terminator
            ? $"{errorMessage} at end."
            : $"{errorMessage} at \"{token.Lexeme}\".";
            
        return new Error(ErrorType.ParseError, token.LineNumber, token.ColumnNumber, errorMessage);
    }

    private IExpr ParseLeftAssociateBinaryOperatorSeries(Func<IExpr> operandExprGetter, params TokenType[] tokenTypes)
    {
        var expr = operandExprGetter();

        while (MatchAnyAndAdvance(tokenTypes))
        {
            var @operator = Previous;
            var right = operandExprGetter();
            expr = new BinaryExpr(expr, @operator, right);
        }

        return expr;
    }
    
    #region Match any TokenType
    private bool MatchAnyAndAdvance(params TokenType[] expectedTokenTypes)
    {
        if (!MatchAny(expectedTokenTypes))
            return false;
        
        Advance();
        return true;
    }
    
    private bool MatchAny(params TokenType[] expectedTokenTypes) => expectedTokenTypes.Any(Match);
    #endregion

    #region Match TokenType
    private bool MatchAndAdvance(TokenType expectedTokenType)
    {
        if (!Match(expectedTokenType))
            return false;
        
        Advance();
        return true;
    }

    private bool Match(TokenType expectedTokenType) => Equals(Current.Type, expectedTokenType);
    #endregion
    
    protected override void Advance(int amount = 1)
    {
        if (!IsAtEnd)
            CurrentIndex += amount;
    }
}