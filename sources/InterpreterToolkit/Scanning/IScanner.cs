using System.Collections.Generic;
using InterpreterToolkit.Tokens;

namespace InterpreterToolkit.Scanning;

public interface IScanner
{
    IEnumerable<Token> ScanTokens();
}