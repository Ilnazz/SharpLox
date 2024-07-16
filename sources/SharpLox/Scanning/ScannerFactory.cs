using SharpLox.Errors;

namespace SharpLox.Scanning;

public class ScannerFactory(IErrorReporter errorReporter) : IScannerFactory
{
    public IScanner CreateScanner(string sourceCode) => new Scanner(errorReporter, sourceCode);
}