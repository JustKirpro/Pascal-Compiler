using System.Collections.Generic;
using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        private static readonly Dictionary<string, Operation> dictionary = new()
        {
            { "IF", Operation.If },
            { "DO", Operation.Do },
            { "OF", Operation.Of },
            { "OR", Operation.Or },
            { "IN", Operation.In },
            { "TO", Operation.To },
            { "END", Operation.End },
            { "VAR", Operation.Var },
            { "DIV", Operation.Div },
            { "AND", Operation.And },
            { "NOT", Operation.Not },
            { "FOR", Operation.For },
            { "MOD", Operation.Mod },
            { "NIL", Operation.Nil },
            { "SET", Operation.Set },
            { "THEN", Operation.Then },
            { "ELSE", Operation.Else },
            { "CASE", Operation.Case },
            { "FILE", Operation.File },
            { "GOTO", Operation.Goto },
            { "TYPE", Operation.Type },
            { "WITH", Operation.With },
            { "BEGIN", Operation.Begin },
            { "WHILE", Operation.While },
            { "ARRAY", Operation.Array },
            { "CONST", Operation.Const },
            { "LABEL", Operation.Label },
            { "UNTIL", Operation.Until },
            { "DOWNTO", Operation.Downto },
            { "PACKED", Operation.Packed },
            { "RECORD", Operation.Record },
            { "REPEAT", Operation.Repeat },
            { "PROGRAM", Operation.Program },
            { "FUNCTION", Operation.Function },
            { "PROCEDURE", Operation.Procedure },
            { "<", Operation.Less },
            { ">", Operation.Greater },
            { "<=", Operation.LessOrEqual },
            { ">=", Operation.GreaterOrEqual },
            { ":=", Operation.Assignment },
            { "+", Operation.Plus },
            { "-", Operation.Minus },
            { "*", Operation.Multiply },
            { "/", Operation.Divide },
            { "=", Operation.Equals },
            { "<>", Operation.NotEqual },
            { "(", Operation.LeftParenthesis },
            { ")", Operation.RightParenthesis },
            { "{", Operation.LeftBracket },
            { "}", Operation.RightBracket },
            { "[", Operation.LeftSquareBracket },
            { "]", Operation.RightSquareBracket },
            { ".", Operation.Point },
            { "..", Operation.TwoPoints },
            { ",", Operation.Comma },
            { ":", Operation.Colon },
            { ";", Operation.Semicolon },
            { "(*", Operation.LeftComment },
            { "*)", Operation.RightComment },
            { "^", Operation.Circumflex }
        };
        readonly IOModule io;
        char currentCharacter;
        
        public LexicalAnalyzer(IOModule io)
        {
            this.io = io;
            currentCharacter = ' ';
        }

        public Token GetNextToken()
        {
            if (!io.HasUnreadCharacter())
                return null;

            while (currentCharacter == ' ')
                currentCharacter = io.ReadNextCharacter();

            StringBuilder stringBuilder = new();

            if (char.IsLetter(currentCharacter))
            {
                while (currentCharacter != ' ' && !IsOperation(currentCharacter.ToString()))
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();

                return IsOperation(token) ? new OperationToken(GetOperation(token)) : new IdentifierToken(token);
            }
            else if (char.IsDigit(currentCharacter))
            {
                while (currentCharacter != ' ' && !IsOperation(currentCharacter.ToString()))
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();

                return token.Contains('.') ? new ConstantToken(float.Parse(token)) : new ConstantToken(int.Parse(token));
            }
            else if (currentCharacter == '\'')
            {
                currentCharacter = io.ReadNextCharacter();

                while (currentCharacter != '\'')
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = io.ReadNextCharacter();
                }

                currentCharacter = io.ReadNextCharacter();

                string token = stringBuilder.ToString();

                return new ConstantToken(token);
            }
            else
            {
                if (currentCharacter == '<' || currentCharacter == '>' || currentCharacter == ':' || currentCharacter == '(' || currentCharacter == '*' || currentCharacter == '.')
                {
                    stringBuilder.Append(currentCharacter);
                    char nextCharacter = io.CheckNextCharacter();
                    stringBuilder.Append(nextCharacter);

                    if (IsOperation(stringBuilder.ToString()))
                    {
                        io.ReadNextCharacter();
                        currentCharacter = io.ReadNextCharacter();
                        return new OperationToken(GetOperation(stringBuilder.ToString()));
                    }
                }

                string token = currentCharacter.ToString();
                currentCharacter = io.ReadNextCharacter();

                return new OperationToken(GetOperation(token));
            }
        }

        private static bool IsOperation(string token) => dictionary.ContainsKey(token.ToUpper());

        private static Operation GetOperation(string token) => dictionary[token.ToUpper()];
    }
}
