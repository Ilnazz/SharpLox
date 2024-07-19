using SharpLox.Expressions;

namespace SharpLox.Utilities;

public interface IExprStringifier
{
    string Stringify(IExpr expr);
}