namespace InterpreterToolkit.Errors;

public interface IErrorReporter
{
    bool WasErrorOccured { get; }

    void ReportError(Error error);
}