using System.Collections.Generic;
using SharpLox.Tokens;

namespace SharpLox.Scanning;

public interface IScanner
{
    IEnumerable<Token> ScanTokens();
}