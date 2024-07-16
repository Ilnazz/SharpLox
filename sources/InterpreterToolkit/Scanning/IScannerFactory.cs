namespace InterpreterToolkit.Scanning;

public interface IScannerFactory
{
    IScanner CreateScanner(string sourceCode);
}