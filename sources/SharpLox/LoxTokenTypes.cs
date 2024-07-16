using InterpreterToolkit.Tokens;

namespace SharpLox;

public static class LoxTokenTypes
{
    public static readonly TokenType
        // Single character tokens.
        LeftParen = new(nameof(LeftParen)), // (
        RightParen = new(nameof(RightParen)), // )

        LeftBrace = new(nameof(LeftBrace)), // {
        RightBrace = new(nameof(RightParen)), // }

        Plus = new(nameof(Plus)), // +
        Minus = new(nameof(Minus)), // -
        Star = new(nameof(Star)), // *
        Slash = new(nameof(Slash)), // /

        // One or two character tokens.
        Bang = new(nameof(Bang)), // !
        BangEqual = new(nameof(BangEqual)), // !=
        Equal = new(nameof(Equal)), // =
        EqualEqual = new(nameof(EqualEqual)), // ==
        Less = new(nameof(Less)), // <
        LessEqual = new(nameof(LessEqual)), // <=
        Greater = new(nameof(Greater)), // >
        GreaterEqual = new(nameof(GreaterEqual)), // >=

        Semicolon = new(nameof(Semicolon)), // ;
        Dot = new(nameof(Dot)), // .
        Comma = new(nameof(Comma)), // ,

        // Literals
        Identifier = new(nameof(Identifier)),
        String = new(nameof(String)),
        Number = new(nameof(Number)),

        // Keywords
        Nil = new(nameof(Nil)),

        False = new(nameof(False)),
        True = new(nameof(True)),

        And = new(nameof(And)),
        Or = new(nameof(Or)),

        Var = new(nameof(Var)),

        If = new(nameof(If)),
        Else = new(nameof(Else)),

        For = new(nameof(For)),
        While = new(nameof(While)),

        Fun = new(nameof(Fun)),
        Return = new(nameof(Return)),

        Class = new(nameof(Class)),
        Super = new(nameof(Super)),
        This = new(nameof(This)),

        Print = new(nameof(Print)),

        // or EOF, EndOfFile, EndOfStream, TerminalToken, LastToken, Last
        Terminator = new(nameof(Terminator));
}