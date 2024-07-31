using SharpLox.Errors;

namespace SharpLox.Interpretation;

public class InterpreterFactory(IEnvironmentFactory environmentFactory, IErrorReporter errorReporter) :
    IInterpreterFactory
{
    public IInterpreter CreateInterpreter() => new Interpreter(environmentFactory, errorReporter);
}