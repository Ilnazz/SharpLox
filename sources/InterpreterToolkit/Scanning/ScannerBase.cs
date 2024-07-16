using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InterpreterToolkit.Errors;
using InterpreterToolkit.Tokens;

namespace InterpreterToolkit.Scanning;

public abstract class ScannerBase(IErrorReporter errorReporter, string sourceCode) : IScanner
{
    #region Protected properties
    protected int CurrentLine { get; private set; } = 1;

    protected int CurrentColumn { get; private set; }

    protected string CurrentLexeme => sourceCode[_lexemeStartIndex.._currentIndex];
    #endregion

    #region Fields
    private int _currentIndex,
                _lexemeStartIndex;
    #endregion

    public IEnumerable<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _lexemeStartIndex = _currentIndex;
            
            var currentChar = PeekAndAdvance();

            if (!HandleChar(currentChar, out var token, out var error))
            {
                errorReporter.ReportError(error);
            }
            else if (token is not null)
            {
                yield return token;
            }
        }

        yield return CreateTerminatorToken();
        
        _currentIndex = 0;
        _lexemeStartIndex = 0;

        CurrentLine = 0;
        CurrentColumn = 0;
    }

    #region Protected methods
    protected abstract bool HandleChar(char c, out Token? token, [NotNullWhen(false)] out Error? error);

    protected abstract Token CreateTerminatorToken();

    protected Token CreateToken(TokenType tokenType, object? literal = null) =>
        new(tokenType, CurrentLine, CurrentColumn, CurrentLexeme, literal);

    protected LexicalError CreateError(string message) =>
        new(CurrentLine, CurrentColumn, message);

    protected bool MatchAndAdvance(char expectedChar)
    {
        if (!Match(expectedChar))
            return false;

        Advance();
        return true;
    }

    protected bool Match(char expectedChar) => Peek() == expectedChar;
    
    protected char PeekAndAdvance(int charsToLookAhead = 0)
    {
        var currentChar = Peek(charsToLookAhead);
        Advance();
        return currentChar;
    }

    protected char Peek(int charsToLookAhead = 0) =>
        _currentIndex + charsToLookAhead < sourceCode.Length
            ? sourceCode[_currentIndex + charsToLookAhead]
            : AsciiChars.Zero;
    
    protected void Advance(int amount = 1)
    {
        if (!IsAtEnd())
            IncreaseCurrentIndexAndColumn(amount);
    }

    protected bool IsAtEnd() => _currentIndex >= sourceCode.Length;

    private void IncreaseCurrentIndexAndColumn(int amount = 1)
    {
        _currentIndex += amount;
        CurrentColumn += amount;
    }
    
    protected void IncrementLineAndResetColumn()
    {
        CurrentLine++;
        CurrentColumn = 0;
    }
    #endregion
}