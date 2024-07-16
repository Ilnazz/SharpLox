namespace SharpLox.Tokens;

public class Token
(
    TokenType type,
    int lineNumber,
    int columnNumber,
    string? lexeme = null,
    object? literal = null)
{
    #region Properties
    public TokenType Type { get; } = type;

    public int LineNumber { get; } = lineNumber;

    public int ColumnNumber { get; } = columnNumber;

    public string? Lexeme { get; } = lexeme;

    public object? Literal { get; } = literal;
    #endregion

    public override string ToString() =>
        $"{LineNumber}:{ColumnNumber} {Type}" +
        $"{(Lexeme is null ? string.Empty : ' ')}{Lexeme}" +
        $"{(Literal is null ? string.Empty : ' ')}{Literal}";
}