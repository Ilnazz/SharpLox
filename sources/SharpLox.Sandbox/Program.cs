using System;
using SharpLox.Expressions;
using SharpLox.Tokens;
using SharpLox.Utilities;

// var expr = new BinaryExpr
// (
//     new UnaryExpr
//     (
//         new Token(TokenType.Minus, 0, 0, "-"),
//         new LiteralExpr(123)
//     ),
//     new Token(TokenType.Star, 0, 0, "*"),
//     new GroupingExpr
//     (
//         new LiteralExpr(10.10)
//     )
// );

var expr = new BinaryExpr
(
    new BinaryExpr
    (
        new LiteralExpr(1),
        new Token(TokenType.Plus, 0, 0, "+"),
        new LiteralExpr(2)
    ),
    new Token(TokenType.Star, 0, 0, "*"),
    new BinaryExpr
    (
        new LiteralExpr(4),
        new Token(TokenType.Minus, 0, 0, "-"),
        new LiteralExpr(3)
    )
);

var exprStringifier = new RpnExprStringifier();

Console.WriteLine(exprStringifier.Stringify(expr));