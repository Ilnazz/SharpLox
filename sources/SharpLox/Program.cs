using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SharpLox.Errors;
using SharpLox.Parsing;
using SharpLox.Scanning;

namespace SharpLox;

public static class Program
{
    public static int Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddSingleton<IErrorReporter, ConsoleErrorReporter>()
            .AddSingleton<IScannerFactory, ScannerFactory>()
            .AddSingleton<IParserFactory, ParserFactory>()
            .AddSingleton<IInterpreterProgram, InterpreterProgram>();

        var serviceProvider = services.BuildServiceProvider();

        var interpreterProgram = serviceProvider.GetRequiredService<IInterpreterProgram>();

        switch (args.Length)
        {
            case > 1:
            {
                Console.WriteLine("Usage: cslox [script]");
                return (int)ProgramExitCode.InvalidUsage;
            }
            case 1:
            {
                var scriptFilePath = args[0];
                var script = File.ReadAllText(scriptFilePath);
                interpreterProgram.Interpret(script);
                break;
            }
            default:
            {
                interpreterProgram.RunPrompt();
                break;
            }
        }

        var errorReporter = serviceProvider.GetRequiredService<IErrorReporter>();
        if (errorReporter.WasErrorOccured)
            return (int)ProgramExitCode.InvalidInput;

        return (int)ProgramExitCode.Success;
    }
}