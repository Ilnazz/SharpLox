namespace SharpLox.Errors;

public interface IErrorReporter
{
    #region Properties
    bool WasLexicalErrorOccured { get; }

    bool WasParseErrorOccured { get; }
    
    bool WasRuntimeErrorOccured { get; }
    #endregion

    #region Methods
    void ReportError(Error error);

    void Reset();
    #endregion
}