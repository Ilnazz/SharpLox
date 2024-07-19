using System;
using System.Linq;
using SharpLox.Expressions;
using SharpLox.Parsing;
using SharpLox.Scanning;
using SharpLox.Utilities;

namespace SharpLox;

public class InterpreterProgram
(
    IScannerFactory scannerFactory,
    IParserFactory parserFactory
)
    : IInterpreterProgram
{
    public void Interpret(string sourceCode)
    {
        var scanner = scannerFactory.CreateScanner(sourceCode);
        var tokens = scanner.ScanTokens().ToList();
        
        Console.WriteLine($"Tokens: {tokens
            .Select(token => $"{token.Type}")
            .Aggregate((f, s) => $"{f}, {s}")}");

        var parser = parserFactory.CreateParser(tokens);
        var expr = parser.Parse();

        if (expr is null)
            return;

        StringifyAndPrintExpr("Lisp-like expr tree: ", new LispLikeExprStringifier(), expr);
        StringifyAndPrintExpr("RPN expr tree: ", new RpnExprStringifier(), expr);
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

    private static void StringifyAndPrintExpr(string header, IExprStringifier exprStringifier, IExpr expr) =>
        Console.WriteLine($"{header} {exprStringifier}: {exprStringifier.Stringify(expr)}");
}