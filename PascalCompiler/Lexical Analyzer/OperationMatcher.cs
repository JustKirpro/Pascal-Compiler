using System.Collections.Generic;

namespace PascalCompiler
{
    public static class OperationMatcher
    {
        private static readonly Dictionary<string, Operation> dictionary = new()
        {
            { "IF", Operation.If },
            { "DO", Operation.Do },
            { "OR", Operation.Or },
            { "IN", Operation.In },
            { "END", Operation.End },
            { "VAR", Operation.Var },
            { "DIV", Operation.Div },
            { "AND", Operation.And },
            { "NOT", Operation.Not },
            { "XOR", Operation.Xor },
            { "MOD", Operation.Mod },
            { "THEN", Operation.Then },
            { "ELSE", Operation.Else },
            { "TYPE", Operation.Type },
            { "BEGIN", Operation.Begin },
            { "WHILE", Operation.While },
            { "PROGRAM", Operation.Program },
            { "<", Operation.Less },
            { ">", Operation.Greater },
            { "<=", Operation.LessOrEqual },
            { ">=", Operation.GreaterOrEqual },
            { ":=", Operation.Assignment },
            { "+", Operation.Plus },
            { "-", Operation.Minus },
            { "*", Operation.Asterisk },
            { "/", Operation.Slash },
            { "=", Operation.Equals },
            { "<>", Operation.NotEqual },
            { "(", Operation.LeftParenthesis },
            { ")", Operation.RightParenthesis },
            { "{", Operation.LeftBracket },
            { "}", Operation.RightBracket },
            { "[", Operation.LeftSquareBracket },
            { "]", Operation.RightSquareBracket },
            { ".", Operation.Point },
            { "..", Operation.TwoPoints},
            { ",", Operation.Comma },
            { ":", Operation.Colon },
            { ";", Operation.Semicolon },
            { "\'", Operation.Quote},
            { "(*", Operation.LeftComment },
            { "*)", Operation.RightComment },
        };

        public static bool IsOperation(string token) => dictionary.ContainsKey(token.ToUpper());

        public static Operation GetOperation(string token) => dictionary[token.ToUpper()];
    }
}
