using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpLox.Base;
using SharpLox.Errors;
using SharpLox.Expressions;
using SharpLox.Scanning;
using SharpLox.Statements;
using SharpLox.Tokens;

namespace SharpLox.Parsing;

public sealed class Parser(IErrorReporter errorReporter, IReadOnlyList<Token> tokens) :
    ScannerParserBase<Token>(errorReporter, tokens), IParser
{
    private bool IsAtEnd => Current.Type is TokenType.Terminator;

    private bool _isSingleUnterminatedExprStmtAllowed;

    #region Public methods
    public ParseResult Parse(bool allowSingleUnterminatedExprStmt)
    {
        var stmts = new List<IStmt>();

        if (allowSingleUnterminatedExprStmt)
        {
            _isSingleUnterminatedExprStmtAllowed = true;

            IExpr? expr = null;
        
            while (!IsAtEnd)
            {
                if (!TryParseDeclStmt(out var stmt))
                    continue;

                if (stmt is ExprStmt exprStmt)
                {
                    expr = exprStmt.Expr;
                    break;
                }
                
                stmts.Add(stmt);
            }

            _isSingleUnterminatedExprStmtAllowed = false;

            if (expr is not null)
                return new ParseResult(ParseResultType.Expression, expr: expr);
            
            return stmts.Count is 0
                ? new ParseResult(ParseResultType.None)
                : new ParseResult(ParseResultType.Statements, stmts: stmts);
        }
        
        while (!IsAtEnd)
        {
            if (TryParseDeclStmt(out var stmt))
                stmts.Add(stmt);
        }

        return stmts.Count is 0
            ? new ParseResult(ParseResultType.None)
            : new ParseResult(ParseResultType.Statements, stmts: stmts);
    }
    #endregion

    #region Statement parsing methods
    private bool TryParseDeclStmt([NotNullWhen(true)] out IStmt? stmt)
    {
        stmt = null;
        
        try
        {
            stmt = MatchAndAdvance(TokenType.Var)
                ? ParseVarDeclStmt()
                : ParseStmt();
        }
        catch (ParseException)
        {
            Synchronize();
        }

        return stmt is not null;
    }

    private IStmt ParseVarDeclStmt()
    {
        var name = Consume(TokenType.Identifier, "expected variable name");

        IExpr? initializer = null;
        if (MatchAndAdvance(TokenType.Equal))
            initializer = ParseExpr();

        ConsumeSemicolonIfNeeded("after variable declaration");
        return new VarStmt(name, initializer);
    }
    
    private IStmt ParseStmt()
    {
        if (MatchAndAdvance(TokenType.Print))
            return ParsePrintStmt();

        if (MatchAndAdvance(TokenType.LeftBrace))
            return new BlockStmt(ParseBlockStmt());

        return ParseExprStmt();
    }

    private IEnumerable<IStmt> ParseBlockStmt()
    {
        var stmts = new List<IStmt>();
        
        while (!IsAtEnd && !Match(TokenType.RightBrace))
        {
            if (TryParseDeclStmt(out var stmt))
                stmts.Add(stmt);
        }

        Consume(TokenType.RightBrace, $"expected \"{AsciiChars.RightBrace}\" after block.");

        return stmts;
    }

    private IStmt ParsePrintStmt()
    {
        var expr = ParseExpr();
        ConsumeSemicolonIfNeeded();
        return new PrintStmt(expr);
    }

    private IStmt ParseExprStmt()
    {
        var expr = ParseExpr();
        ConsumeSemicolonIfNeeded();
        return new ExprStmt(expr);
    }

    private void ConsumeSemicolonIfNeeded(string? place = null)
    {
        if (_isSingleUnterminatedExprStmtAllowed && IsAtEnd)
            return;
        
        var errorMessage = $"expected \"{AsciiChars.Semicolon}\"";
        if (place is not null)
            errorMessage = $"{errorMessage} {place}";
            
        Consume(TokenType.Semicolon, errorMessage);
    }
    #endregion

    #region Expression parsing methods
    private IExpr ParseExpr() =>
        ParseLeftAssociateBinaryOperatorSeries(ParseAssignment, TokenType.Comma);

    private IExpr ParseAssignment()
    {
        var expr = ParseConditional();

        if (MatchAndAdvance(TokenType.Equal))
        {
            var equals = Previous;
            var value = ParseAssignment();

            if (expr is VarExpr varExpr)
            {
                var name = varExpr.Name;
                return new AssignExpr(name, value);
            }
            
            ReportError(CreateError(equals, "Invalid assignment target."));
        }

        return expr;
    }
    
    private IExpr ParseConditional()
    {
        var expr = ParseEquality();

        if (!MatchAndAdvance(TokenType.Question))
            return expr;

        var questionOperator = Previous;
        
        var then = ParseEquality();
        Consume(TokenType.Colon, $"expected \"{AsciiChars.Colon}\" in conditional expression");
        var @else = ParseEquality();
        return new ConditionalExpr(questionOperator, expr, then, @else);
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
        
        if (MatchAndAdvance(TokenType.True))
            return new LiteralExpr(true);
        
        if (MatchAndAdvance(TokenType.Nil))
            return new LiteralExpr(null);
        
        if (MatchAndAdvance(TokenType.Identifier))
            return new VarExpr(Previous);

        if (MatchAnyAndAdvance(TokenType.Number, TokenType.String))
            return new LiteralExpr(Previous.Literal);

        if (MatchAndAdvance(TokenType.LeftParen))
        {
            var expr = ParseExpr();
            Consume(TokenType.RightParen, $"expected \"{AsciiChars.RightParen}\" after expression");
            return new GroupingExpr(expr);
        }
        
        ReportError(CreateError(Current, "expression expected"));
        throw new ParseException();
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
    #endregion

    #region Utility methods
    private Token Consume(TokenType tokenType, string errorMessage)
    {
        if (Match(tokenType))
        {
            var token = Current;
            Advance();
            return token;
        }

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
    #endregion

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