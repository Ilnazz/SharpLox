using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public sealed class ZeroDivisionException(Token token, string message) : RuntimeException(token, message);