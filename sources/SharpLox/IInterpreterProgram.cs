namespace SharpLox;

public interface IInterpreterProgram
{
    void Interpret(string sourceCode);

    void RunPrompt();
}