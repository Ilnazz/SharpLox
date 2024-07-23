using SharpLox.Errors;

namespace SharpLox.Interpretation;

public class InterpreterFactory(IErrorReporter errorReporter) : IInterpreterFactory
{
    public IInterpreter CreateInterpreter() => new Interpreter(errorReporter);
}