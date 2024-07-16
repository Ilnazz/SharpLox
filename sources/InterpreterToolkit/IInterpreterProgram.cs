namespace InterpreterToolkit;

public interface IInterpreterProgram
{
    void Interpret(string sourceCode);

    void RunPrompt();
}