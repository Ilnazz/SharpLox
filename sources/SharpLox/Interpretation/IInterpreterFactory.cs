namespace SharpLox.Interpretation;

public interface IInterpreterFactory
{
    IInterpreter CreateInterpreter();
}