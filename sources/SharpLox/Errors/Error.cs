namespace SharpLox.Errors;

public sealed class Error(ErrorType type, int lineNumber, int columnNumber, string message)
{
    public ErrorType Type { get; } = type;

    public int LineNumber { get; } = lineNumber;

    public int ColumnNumber { get; } = columnNumber;

    public string Message { get; } = message;

    public override string ToString() =>
        $"{LineNumber}:{ColumnNumber} " +
        $"{(Type is ErrorType.LexicalError ? "Lexical error" : "Parse error")}: " +
        $"{Message}";
}