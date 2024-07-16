﻿using System;
using InterpreterToolkit.Scanning;

namespace InterpreterToolkit;

public abstract class InterpreterProgramBase(IScannerFactory scannerFactory) : IInterpreterProgram
{
    public void Interpret(string sourceCode)
    {
        var scanner = scannerFactory.CreateScanner(sourceCode);

        var tokens = scanner.ScanTokens();
        foreach (var token in tokens)
            Console.WriteLine(token);
    }

    public void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");

            var sourceCodeLine = Console.ReadLine();
            if (sourceCodeLine is null)
                break;

            Interpret(sourceCodeLine);
        }
    }
}