using InterpreterToolkit.Errors;
using InterpreterToolkit.Scanning;

namespace SharpLox.Scanning;

public class LoxScannerFactory(IErrorReporter errorReporter) : IScannerFactory
{
    public IScanner CreateScanner(string sourceCode) => new LoxScanner(errorReporter, sourceCode);
}