namespace SharpLox.Interpretation;

public sealed class EnvironmentFactory : IEnvironmentFactory
{
    public IEnvironment CreateEnvironment(IEnvironment? enclosing) =>
        new Environment(enclosing);
}