using System.Collections.Generic;
using SharpLox.Errors;
using SharpLox.Tokens;

namespace SharpLox.Parsing;

public class ParserFactory(IErrorReporter errorReporter) : IParserFactory
{
    public IParser CreateParser(IReadOnlyList<Token> tokens) =>
        new Parser(errorReporter, tokens);
}