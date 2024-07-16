namespace SharpLox.Expressions;

public interface IExprStringifier
{
    string Stringify(IExpr expr);
}