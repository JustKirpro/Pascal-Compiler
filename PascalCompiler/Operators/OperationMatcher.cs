using System.Collections.Generic;

namespace PascalCompiler
{
    public static class OperationMatcher
    {
        private static readonly Dictionary<string, Operation> dictionary = new Dictionary<string, Operation>()
        {
            ["IF"] = Operation.If,
            ["DO"] = Operation.Do,
            ["OF"] = Operation.Of,
            ["OR"] = Operation.Or,
            ["IN"] = Operation.In,
            ["TO"] = Operation.To,
            ["END"] = Operation.End,
            ["VAR"] = Operation.Var,
            ["DIV"] = Operation.Div,
            ["AND"] = Operation.And,
            ["NOT"] = Operation.Not,
            ["FOR"] = Operation.For,
            ["XOR"] = Operation.Xor,
            ["MOD"] = Operation.Mod,
            ["NIL"] = Operation.Nil,
            ["SET"] = Operation.Set,
            ["THEN"] = Operation.Then,
            ["ELSE"] = Operation.Else,
            ["CASE"] = Operation.Case,
            ["FILE"] = Operation.File,
            ["GOTO"] = Operation.Goto,
            ["TYPE"] = Operation.Type,
            ["WITH"] = Operation.With,
            ["BEGIN"] = Operation.Begin,
            ["WHILE"] = Operation.While,
            ["ARRAY"] = Operation.Array,
            ["CONST"] = Operation.Const,
            ["LABEL"] = Operation.Label,
            ["UNTIL"] = Operation.Until,
            ["DOWNTO"] = Operation.Downto,
            ["PACKED"] = Operation.Packed,
            ["RECORD"] = Operation.Record,
            ["REPEAT"] = Operation.Repeat,
            ["PROGRAM"] = Operation.Program,
            ["<"] = Operation.Less,
            [">"] = Operation.Greater,
            ["<="] = Operation.LessOrEqual,
            [">="] = Operation.GreaterOrEqual,
            [":="] = Operation.Assignment,
            ["+"] = Operation.Plus,
            ["-"] = Operation.Minus,
            ["*"] = Operation.Asterisk,
            ["/"] = Operation.Slash,
            ["="] = Operation.Equals,
            ["<>"] = Operation.NotEqual,
            ["("] = Operation.LeftParenthesis,
            [")"] = Operation.RightParenthesis,
            ["["] = Operation.LeftSquareBracket,
            ["]"] = Operation.RightSquareBracket,
            ["."] = Operation.Point,
            [".."] = Operation.TwoPoints,
            [","] = Operation.Comma,
            [":"] = Operation.Colon,
            [";"] = Operation.Semicolon,
            ["^"] = Operation.Circumflex
        };

        public static bool IsOperation(string token) => dictionary.ContainsKey(token.ToUpper());

        public static Operation GetOperation(string token) => dictionary[token.ToUpper()];
    }
}