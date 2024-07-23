namespace SharpLox.Errors;

public interface IErrorReporter
{
    #region Properties
    bool WasLexicalErrorOccured { get; }

    bool WasParseErrorOccured { get; }
    
    bool WasRuntimeErrorOccured { get; }
    #endregion

    void ReportError(Error error);
}