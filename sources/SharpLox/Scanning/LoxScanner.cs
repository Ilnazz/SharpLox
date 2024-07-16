using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using InterpreterToolkit.Errors;
using InterpreterToolkit.Scanning;
using InterpreterToolkit.Tokens;

namespace SharpLox.Scanning;

public class LoxScanner(IErrorReporter errorReporter, string sourceCode) : ScannerBase(errorReporter, sourceCode)
{
    #region Fields
    private static readonly CultureInfo EnglishCulture = new("en-US");
    
    private static readonly FrozenDictionary<string, TokenType> TokenTypeByKeywords = new Dictionary<string, TokenType>
    {
        [LoxKeywords.Nil] = LoxTokenTypes.Nil,
        
        [LoxKeywords.True] = LoxTokenTypes.True,
        [LoxKeywords.False] = LoxTokenTypes.False,
    
        [LoxKeywords.And] = LoxTokenTypes.And,
        [LoxKeywords.Or] = LoxTokenTypes.Or,
        
        [LoxKeywords.Var] = LoxTokenTypes.Var,
        
        [LoxKeywords.If] = LoxTokenTypes.If,
        [LoxKeywords.Else] = LoxTokenTypes.Else,
        
        [LoxKeywords.Print] = LoxTokenTypes.Print,
        
        [LoxKeywords.Fun] = LoxTokenTypes.Fun,
        [LoxKeywords.Return] = LoxTokenTypes.Return,
        
        [LoxKeywords.Class] = LoxTokenTypes.Class,
        [LoxKeywords.Super] = LoxTokenTypes.Super,
        [LoxKeywords.This] = LoxTokenTypes.This,
        
        [LoxKeywords.While] = LoxTokenTypes.While,
        [LoxKeywords.For] = LoxTokenTypes.For
    }
        .ToFrozenDictionary();
    #endregion

    protected override bool HandleChar(char c, out Token? token, [NotNullWhen(false)] out Error? error)
    {
        token = null;
        error = null;
        
        switch (c)
        {
            case AsciiChars.LeftParen:
            {
                token = CreateToken(LoxTokenTypes.LeftParen);
                break;
            }
            case AsciiChars.RightParen:
            {
                token = CreateToken(LoxTokenTypes.RightParen);
                break;
            }

            case AsciiChars.LeftBrace:
            {
                token = CreateToken(LoxTokenTypes.LeftBrace);
                break;
            }
            case AsciiChars.RightBrace:
            {
                token = CreateToken(LoxTokenTypes.RightBrace);
                break;
            }

            case AsciiChars.Dot:
            {
                token = CreateToken(LoxTokenTypes.Dot);
                break;
            }
            case AsciiChars.Comma:
            {
                token = CreateToken(LoxTokenTypes.Comma);
                break;
            }
            case AsciiChars.Semicolon:
            {
                token = CreateToken(LoxTokenTypes.Semicolon);
                break;
            }
            
            case AsciiChars.Plus:
            {
                token = CreateToken(LoxTokenTypes.Plus);
                break;
            }
            case AsciiChars.Minus:
            {
                token = CreateToken(LoxTokenTypes.Minus);
                break;
            }
            case AsciiChars.Star:
            {
                token = CreateToken(LoxTokenTypes.Star);
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
                    ? LoxTokenTypes.BangEqual : LoxTokenTypes.Bang);
                break;
            }
            case AsciiChars.Equal:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? LoxTokenTypes.EqualEqual : LoxTokenTypes.Equal);
                break;
            }
            case AsciiChars.Less:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? LoxTokenTypes.LessEqual : LoxTokenTypes.Less);
                break;
            }
            case AsciiChars.Greater:
            {
                token = CreateToken(MatchAndAdvance(AsciiChars.Equal)
                    ? LoxTokenTypes.GreaterEqual : LoxTokenTypes.Greater);
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
                    error = CreateError("Unexpected character");
                }
                
                break;
            }
        }

        return error is null;
    }

    protected override Token CreateTerminatorToken() =>
        new(LoxTokenTypes.Terminator, CurrentLine, CurrentColumn);

    #region Character handlers
    private void HandleWhiteSpace() => IncrementLineAndResetColumn();

    private void HandleSlash(out Token? token, out Error? error)
    {
        token = null;
        error = null;
        
        // A single line comment.
        if (Match(AsciiChars.Slash))
        {
            Advance();
            
            while (!IsAtEnd() && Peek() is not AsciiChars.NewLine)
                Advance();
        }
        // A multi-line comment.
        else if (Match(AsciiChars.Star))
        {
            Advance();
            
            while (!IsAtEnd() &&
                   !(Peek() is AsciiChars.Star && PeekNext() is AsciiChars.Slash))
            {
                if (PeekAndAdvance() is AsciiChars.NewLine)
                    IncrementLineAndResetColumn();
            }
            
            if (IsAtEnd())
            {
                error = CreateError("Unterminated multi-line comment");
                return;
            }

            // The star and slash.
            Advance(amount: 2);
        }
        // A single slash.
        else
        {
            token = CreateToken(LoxTokenTypes.Slash);
        }
    }
    
    private void HandleQuote(out Token? token, out Error? error)
    {
        token = null;
        error = null;
        
        while (!IsAtEnd() && Peek() is not AsciiChars.Quote)
            if (PeekAndAdvance() is AsciiChars.NewLine)
                IncrementLineAndResetColumn();

        if (IsAtEnd())
        {
            error = CreateError("Unterminated string");
            return;
        }
        
        // The closing quote.
        Advance();

        var literal = CurrentLexeme[1..^2];
        token = CreateToken(LoxTokenTypes.String, literal);
    }

    #region Number handling
    private void HandleDigit(out Token? token)
    {
        token = null;

        AdvanceWhilePeekingDigit();

        if (Peek() is AsciiChars.Dot && IsDigit(PeekNext()))
        {
            // The number decimal point.
            Advance();

            AdvanceWhilePeekingDigit();
        }

        var englishCultureNumberFormat = EnglishCulture.NumberFormat;
        var literal = decimal.Parse(CurrentLexeme, englishCultureNumberFormat);
        token = CreateToken(LoxTokenTypes.Number, literal);
    }

    private void AdvanceWhilePeekingDigit()
    {
        while (IsDigit(Peek()))
            Advance();
    }
    #endregion

    #region Identifiers and keywords handling
    private void HandleAlpha(out Token? token)
    {
        token = null;

        AdvanceWhilePeekingAlphaOrDigit();

        var tokenType = TokenTypeByKeywords.GetValueOrDefault(CurrentLexeme, LoxTokenTypes.Identifier);
        token = CreateToken(tokenType);
    }

    private void AdvanceWhilePeekingAlphaOrDigit()
    {
        while (IsAlphaOrDigit(Peek()))
            Advance();
    }
    #endregion
    #endregion

    private char PeekNext() => Peek(charsToLookAhead: 1);

    #region Char helpers
    private static bool IsWhiteSpace(char c) =>
        c is AsciiChars.Space
        or AsciiChars.Tabulation
        or AsciiChars.CarriageReturn;

    private static bool IsAlphaOrDigit(char c) => IsDigit(c) || IsAlpha(c);
    
    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    private static bool IsAlpha(char c) =>
        c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    #endregion
}