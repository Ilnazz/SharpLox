namespace SharpLox.Parsing;

public interface IParser
{
    ParseResult Parse(bool allowSingleUnterminatedExprStmt);
}