using System;

namespace InterpreterToolkit.Errors;

public class ConsoleErrorReporter : IErrorReporter
{
    public bool WasErrorOccured { get; private set; }

    public void ReportError(Error error)
    {
        WasErrorOccured = true;
        
        Console.Error.WriteLine(error);
    }
}