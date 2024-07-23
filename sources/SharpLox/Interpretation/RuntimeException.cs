using System;
using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public class RuntimeException(Token token, string message) : Exception(message)
{
    public Token Token { get; } = token;
}