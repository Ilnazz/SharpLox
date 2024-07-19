using System.Collections.Generic;
using SharpLox.Tokens;

namespace SharpLox.Parsing;

public interface IParserFactory
{
    IParser CreateParser(IReadOnlyList<Token> tokens);
}