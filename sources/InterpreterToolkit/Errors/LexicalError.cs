namespace InterpreterToolkit.Errors;

public class LexicalError(int lineNumber, int columnNumber, string message) : Error
{
    public int LineNumber { get; } = lineNumber;

    public int ColumnNumber { get; } = columnNumber;

    public string Message { get; } = message;

    public override string ToString() => $"Lexical error: {Message}";
}