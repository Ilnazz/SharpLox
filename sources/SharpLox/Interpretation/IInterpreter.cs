using SharpLox.Expressions;

namespace SharpLox.Interpretation;

public interface IInterpreter
{
    void Interpret(IExpr expr);
}