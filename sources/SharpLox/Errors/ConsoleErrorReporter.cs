using System;

namespace SharpLox.Errors;

public class ConsoleErrorReporter : IErrorReporter
{
    #region Properties
    public bool WasLexicalErrorOccured { get; private set; }
    
    public bool WasParseErrorOccured { get; private set; }
    
    public bool WasRuntimeErrorOccured { get; private set; }
    #endregion

    public void ReportError(Error error)
    {
        switch (error.Type)
        {
            case ErrorType.LexicalError:
            {
                WasLexicalErrorOccured = true;
                break;
            }
            case ErrorType.ParseError:
            {
                WasParseErrorOccured = true;
                break;
            }
            case ErrorType.RuntimeError:
            {
                WasRuntimeErrorOccured = true;
                break;
            }
        }

        Console.Error.WriteLine(error);
    }
}