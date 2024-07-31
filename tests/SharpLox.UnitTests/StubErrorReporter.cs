using SharpLox.Errors;

namespace SharpLox.UnitTests;

public class StubErrorReporter : IErrorReporter
{
    public bool WasLexicalErrorOccured { get; private set; }
    
    public bool WasParseErrorOccured { get; private set; }
    
    public bool WasRuntimeErrorOccured { get; private set; }
    
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
    }

    public void Reset()
    {
        // Todo: delete UnitTests and Sandbox projects...
    }
}