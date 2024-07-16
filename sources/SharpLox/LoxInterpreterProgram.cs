using InterpreterToolkit;
using InterpreterToolkit.Scanning;

namespace SharpLox;

public class LoxInterpreterProgram(IScannerFactory scannerFactory) :
    InterpreterProgramBase(scannerFactory);