namespace SharpLox.Tokens;

public enum TokenType
{
    // Single character tokens.
    LeftParen, // (
    RightParen, // )

    LeftBrace, // {
    RightBrace, // }

    Plus, // +
    Minus, // -
    Star, // *
    Slash, // /

    // One or two character tokens.
    Bang, // !
    BangEqual, // !=
    Equal, // =
    EqualEqual, // ==
    Less, // <
    LessEqual, // <=
    Greater, // >
    GreaterEqual, // >=

    Semicolon, // ;
    Dot, // .
    Comma, // ,
    Question, // ?
    Colon, // :

    // Literals
    Identifier,
    String,
    Number,

    // Keywords
    Nil,

    False,
    True,

    And,
    Or,

    Var,

    If,
    Else,

    For,
    While,

    Fun,
    Return,

    Class,
    Super,
    This,

    Print,

    // or EOF, EndOfFile, EndOfStream, TerminalToken, LastToken, Last
    Terminator
}