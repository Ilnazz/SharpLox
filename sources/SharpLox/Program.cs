using System;
using System.IO;
using InterpreterToolkit;
using InterpreterToolkit.Errors;
using InterpreterToolkit.Scanning;
using Microsoft.Extensions.DependencyInjection;
using SharpLox.Scanning;

namespace SharpLox;

public class Program
{
    public static int Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddSingleton<IErrorReporter, ConsoleErrorReporter>()
            .AddSingleton<IScannerFactory, LoxScannerFactory>()
            .AddSingleton<IInterpreterProgram, LoxInterpreterProgram>();

        var serviceProvider = services.BuildServiceProvider();

        var interpreterProgram = serviceProvider.GetRequiredService<IInterpreterProgram>();

        ProgramExitCode exitCode = ProgramExitCode.Success;

        if (args.Length > 1)
        {
            Console.WriteLine("Usage: sharp-lox [script]"); // cs-lox, cslox, sharplox, SharpLox, CsLox
            exitCode = ProgramExitCode.InvalidUsage;
        }
        else if (args.Length is 1)
        {
            var scriptFilePath = args[0];
            var script = File.ReadAllText(scriptFilePath);
            interpreterProgram.Interpret(script);
        }
        else
        {
            interpreterProgram.RunPrompt();
        }

        // This is the first part of our compiler/interpreter conveyor.

        var errorReporter = serviceProvider.GetRequiredService<IErrorReporter>();
        if (errorReporter.WasErrorOccured)
            exitCode = ProgramExitCode.InvalidInput;

        return (int)exitCode;
    }
}