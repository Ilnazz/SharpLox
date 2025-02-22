﻿using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SharpLox.Base;
using SharpLox.Errors;
using SharpLox.Tokens;

namespace SharpLox.Scanning;

public sealed class Scanner(IErrorReporter errorReporter, string sourceCode) :
    ScannerParserBase<char>(errorReporter, sourceCode.ToCharArray()), IScanner
{
    #region Private properties
    // It is important to use '>' instead of '>=', otherwise the last item will not be handled...
    private bool IsAtEnd => CurrentIndex > sourceCode.Length - 1;
    
    private string CurrentLexeme => sourceCode[_lexemeStartIndex .. CurrentIndex];
    #endregion
    
    #region Fields
    // Needed to parse double with '.' char
    private static readonly CultureInfo EnglishCulture = new("en-US");
    
    private static readonly FrozenDictionary<string, TokenType> TokenTypeByKeywords = new Dictionary<string, TokenType>
    {
        [Keywords.Nil] = TokenType.Nil,
        
        [Keywords.True] = TokenType.True,
        [Keywords.False] = TokenType.False,
    
        [Keywords.And] = TokenType.And,
        [Keywords.Or] = TokenType.Or,
        
        [Keywords.Var] = TokenType.Var,
        
        [Keywords.If] = TokenType.If,
        [Keywords.Else] = TokenType.Else,
        
        [Keywords.Print] = TokenType.Print,
        
        [Keywords.Fun] = TokenType.Fun,
        [Keywords.Return] = TokenType.Return,
        
        [Keywords.Class] = TokenType.Class,
        [Keywords.Super] = TokenType.Super,
        [Keywords.This] = TokenType.This,
        
        [Keywords.While] = TokenType.While,
        [Keywords.For] = TokenType.For
    }
        .ToFrozenDictionary();
    
    private int
        _currentLine,
        _currentColumn,
        _lexemeStartIndex;
    #endregion
    
    public IEnumerable<Token> ScanTokens()
    {
        if (!IsAtEnd)
            _currentLine = 1;
        
        while (!IsAtEnd)
        {
            _lexemeStartIndex = CurrentIndex;
            
            var currentChar = PeekAndAdvance();

            if (!HandleChar(currentChar, out var token, out var error))
            {
                ReportError(error);
            }
            else if (token is not null)
            {
                yield return token;
            }
        }

        yield return CreateTerminatorToken();
        
        CurrentIndex = 0;
        _lexemeStartIndex = 0;

        _currentLine = 0;
        _currentColumn = 0;
    }
    
    #region Char handling
    private bool HandleChar(char c, out Token? token, [NotNullWhen(false)] out Error? error)
    {
        token = null;
        error = null;
        
        switch (c)
        {
            case AsciiChars.LeftParen:
            {
                token = CreateToken(TokenType.LeftParen);
                break;
            }
            case AsciiChars.RightParen:
            {
                token = CreateToken(TokenType.RightParen);
                break;
            }

            case AsciiChars.LeftBrace:
            {
                token = CreateToken(TokenType.LeftBrace);
                break;
            }
            case AsciiChars.RightBrace:
            {
                token = CreateToken(TokenType.RightBrace);
                break;
            }

            case AsciiChars.Dot:
            {
                token = CreateToken(TokenType.Dot);
                break;
            }
            case AsciiChars.Comma:
            {
                token = CreateToken(TokenType.Comma);
                break;
            }
            case AsciiChars.Semicolon:
            {
                token = CreateToken(TokenType.Semicolon);
                break;
            }
            case AsciiChars.Question:
            {
                token = CreateToken(TokenType.Question);
                break;
            }
            case AsciiChars.Colon:
            {
                token = CreateToken(TokenType.Colon);
                break;
            }
            
            case AsciiChars.Plus:
            {
                token = CreateToken(TokenType.Plus);
                break;
            }
            case AsciiChars.Minus:
            {
                token = CreateToken(TokenType.Minus);
                break;
            }
            case AsciiChars.Star:
            {
                token = CreateToken(TokenType.Star);
                break;
            }
            case AsciiChars.Slash:
            {
                HandleSlash(out token, out error);
                break;
            }
            
            case AsciiChars.Bang:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? TokenType.BangEqual : TokenType.Bang);
                break;
            }
            case AsciiChars.Equal:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? TokenType.EqualEqual : TokenType.Equal);
                break;
            }
            case AsciiChars.Less:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? TokenType.LessEqual : TokenType.Less);
                break;
            }
            case AsciiChars.Greater:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            }
            
            case AsciiChars.Quote:
            {
                HandleQuote(out token, out error);
                break;
            }

            case AsciiChars.NewLine:
            {
                IncrementLineAndResetColumn();
                break;
            }
            
            default:
            {
                if (IsWhiteSpace(c))
                {
                    HandleWhiteSpace();
                }
                else if (IsDigit(c))
                {
                    HandleDigit(out token);
                }
                else if (IsAlpha(c))
                {
                    HandleAlpha(out token);
                }
                else
                {
                    error = CreateError($"unexpected character \"{c}\".");
                }
                
                break;
            }
        }

        return error is null;
    }

    private void HandleWhiteSpace() => _currentColumn++;

    private void HandleSlash(out Token? token, out Error? error)
    {
        token = null;
        error = null;
        
        // A single line comment.
        if (Match(AsciiChars.Slash))
        {
            Advance();
            
            while (!IsAtEnd && Peek() is not AsciiChars.NewLine)
                Advance();
        }
        // A multi-line comment.
        else if (Match(AsciiChars.Star))
        {
            Advance();
            
            while (!IsAtEnd &&
                   !(Peek() is AsciiChars.Star && Next is AsciiChars.Slash))
            {
                if (PeekAndAdvance() is AsciiChars.NewLine)
                    IncrementLineAndResetColumn();
            }
            
            if (IsAtEnd)
            {
                error = CreateError("unterminated multi-line comment.");
                return;
            }

            // The star and slash.
            AdvanceTwice();
        }
        // A single slash.
        else
        {
            token = CreateToken(TokenType.Slash);
        }
    }
    
    private void HandleQuote(out Token? token, out Error? error)
    {
        token = null;
        error = null;
        
        while (!IsAtEnd && Peek() is not AsciiChars.Quote)
            if (PeekAndAdvance() is AsciiChars.NewLine)
                IncrementLineAndResetColumn();

        if (IsAtEnd)
        {
            error = CreateError("unterminated string.");
            return;
        }
        
        // The closing quote.
        Advance();

        var literal = CurrentLexeme[1..^1];
        token = CreateToken(TokenType.String, literal);
    }

    #region Number handling
    private void HandleDigit(out Token? token)
    {
        token = null;

        AdvanceWhilePeekingDigit();

        if (Peek() is AsciiChars.Dot && IsDigit(Next))
        {
            // The number decimal point.
            Advance();

            AdvanceWhilePeekingDigit();
        }

        var englishCultureNumberFormat = EnglishCulture.NumberFormat;
        var literal = double.Parse(CurrentLexeme, englishCultureNumberFormat);
        token = CreateToken(TokenType.Number, literal);
    }

    private void AdvanceWhilePeekingDigit()
    {
        while (!IsAtEnd && IsDigit(Peek()))
            Advance();
    }
    #endregion

    #region Identifiers and keywords handling
    private void HandleAlpha(out Token? token)
    {
        token = null;

        AdvanceWhilePeekingAlphaOrDigit();

        var tokenType = TokenTypeByKeywords.GetValueOrDefault(CurrentLexeme, TokenType.Identifier);
        token = CreateToken(tokenType);
    }

    private void AdvanceWhilePeekingAlphaOrDigit()
    {
        while (!IsAtEnd && IsAlphaOrDigit(Peek()))
            Advance();
    }
    #endregion
    #endregion

    #region Char helpers
    private static bool IsWhiteSpace(char c) =>
        c is AsciiChars.Space
        or AsciiChars.Tabulation
        or AsciiChars.CarriageReturn;

    private static bool IsAlphaOrDigit(char c) =>
        IsDigit(c) || IsAlpha(c);
    
    private static bool IsDigit(char c) =>
        c is >= '0' and <= '9';

    private static bool IsAlpha(char c) =>
        c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';    
    #endregion
    
    #region Creational methods
    private Token CreateToken(TokenType tokenType, object? literal = null) =>
        new(tokenType, _currentLine, _currentColumn, CurrentLexeme, literal);

    private Token CreateTerminatorToken() =>
        new(TokenType.Terminator, _currentLine, _currentColumn);

    private Error CreateError(string message) =>
        new(ErrorType.LexicalError, _currentLine, _currentColumn, message);
    #endregion

    #region Advancement methods
    private void AdvanceTwice() => Advance(amount: 2);
    
    protected override void Advance(int amount = 1)
    {
        if (IsAtEnd)
            return;
        
        CurrentIndex += amount;
        _currentColumn += amount;
    }
    #endregion

    private void IncrementLineAndResetColumn()
    {
        _currentLine++;
        _currentColumn = 0;
    }
}