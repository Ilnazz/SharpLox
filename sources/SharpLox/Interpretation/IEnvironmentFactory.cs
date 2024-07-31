namespace SharpLox.Interpretation;

public interface IEnvironmentFactory
{
    IEnvironment CreateEnvironment(IEnvironment? enclosing);
}