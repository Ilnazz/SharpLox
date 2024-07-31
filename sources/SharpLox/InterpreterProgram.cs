using System;
using System.Linq;
using SharpLox.Errors;
using SharpLox.Interpretation;
using SharpLox.Parsing;
using SharpLox.Scanning;

namespace SharpLox;

public class InterpreterProgram
(
    IErrorReporter errorReporter,
    IScannerFactory scannerFactory,
    IParserFactory parserFactory,
    IInterpreterFactory interpreterFactory
)
    : IInterpreterProgram
{
    private const string PromptString = "> ";

    #region Fields
    private readonly IInterpreter _interpreter = interpreterFactory.CreateInterpreter();

    private bool _isInterpretingFromPrompt;
    #endregion

    #region Public methods
    public void Interpret(string sourceCode)
    {
        var scanner = scannerFactory.CreateScanner(sourceCode);
        var tokens = scanner.ScanTokens().ToArray();
        
        var parser = parserFactory.CreateParser(tokens);
        var parseResult = parser.Parse(allowSingleUnterminatedExprStmt: _isInterpretingFromPrompt);

        // Todo: what about checking parseResult?
        if (errorReporter.WasLexicalErrorOccured || errorReporter.WasParseErrorOccured)
        {
            errorReporter.Reset();
            return;
        }
        
        if (parseResult.Type is ParseResultType.Expression)
            _interpreter.Interpret(parseResult.Expr!);
        else if (parseResult.Type is ParseResultType.Statements)
            _interpreter.Interpret(parseResult.Stmts!);
    }

    public void RunPrompt()
    {
        _isInterpretingFromPrompt = true;
        
        while (true)
        {
            Console.Write(PromptString);

            var sourceCodeLine = Console.ReadLine();
            if (sourceCodeLine is null)
                break;

            Interpret(sourceCodeLine);
        }

        _isInterpretingFromPrompt = false;
    }
    #endregion
    
    // StringifyAndPrintExpr("Lisp-like expr tree: ", new LispLikeExprStringifier(), expr);
    // StringifyAndPrintExpr("RPN expr tree: ", new RpnExprStringifier(), expr);
    // private static void StringifyAndPrintExpr(string header, IExprStringifier exprStringifier, IExpr expr) =>
    //     Console.WriteLine($"{header} {exprStringifier}: {exprStringifier.Stringify(expr)}");
}