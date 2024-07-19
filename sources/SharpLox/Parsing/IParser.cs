using SharpLox.Expressions;

namespace SharpLox.Parsing;

public interface IParser
{
    IExpr? Parse();
}