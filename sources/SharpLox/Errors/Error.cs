using System.ComponentModel;

namespace SharpLox.Errors;

public sealed class Error(ErrorType type, int lineNumber, int columnNumber, string message)
{
    #region Properties
    public ErrorType Type { get; } = type;

    public int LineNumber { get; } = lineNumber;

    public int ColumnNumber { get; } = columnNumber;

    public string Message { get; } = message;
    #endregion

    public override string ToString() =>
        $"{LineNumber}:{ColumnNumber} " +
        $"{StringifyErrorType(Type)}: " +
        $"{Message}";

    private static string StringifyErrorType(ErrorType errorType) => errorType switch
    {
        ErrorType.LexicalError => "Lexical error",
        ErrorType.ParseError => "Parse error",
        ErrorType.RuntimeError => "Runtime error",
        _ => throw new InvalidEnumArgumentException(nameof(errorType), (int)errorType, typeof(ErrorType))
    };
}