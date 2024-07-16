namespace InterpreterToolkit.Tokens;

public class TokenType(string name)
{
    public string Name { get; } = name;

    public override string ToString() => Name;
}