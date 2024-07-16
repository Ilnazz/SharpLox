namespace SharpLox.Scanning;

public interface IScannerFactory
{
    IScanner CreateScanner(string sourceCode);
}