using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public interface IEnvironment
{
    void Define(string name);

    void Assign(Token name, object? value);

    object? Get(Token name);
}