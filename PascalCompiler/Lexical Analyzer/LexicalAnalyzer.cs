using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        readonly IOModule io;
        char currentCharacter;
        
        public LexicalAnalyzer(string inputPath, string outputPath)
        {
            io = new(inputPath, outputPath);
            currentCharacter = ' ';
        }

        public Token GetNextToken()
        {
            if (!io.HasUnreadCharacter())
                return null;

            while (currentCharacter == ' ')
                currentCharacter = io.ReadNextCharacter();

            if (currentCharacter == '{')
            {
                while (currentCharacter != '}')
                    currentCharacter = io.ReadNextCharacter();

                currentCharacter = io.ReadNextCharacter();
            }

            while (currentCharacter == ' ')
                currentCharacter = io.ReadNextCharacter();

            StringBuilder stringBuilder = new();

            if (char.IsLetter(currentCharacter) || currentCharacter == '_')
            {
                while (currentCharacter != ' ' && !OperationMatcher.IsOperation(currentCharacter.ToString()))
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();
                CheckForbiddenSymbol(token);
               
                return OperationMatcher.IsOperation(token) ? new OperationToken(OperationMatcher.GetOperation(token)) : new IdentifierToken(token);
            }
            else if (char.IsDigit(currentCharacter))
            {
                while (currentCharacter != ' ' && (!OperationMatcher.IsOperation(currentCharacter.ToString()) || currentCharacter == '.'))
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();

                if (token.Contains('.') || token.Contains('E') || token.Contains('e'))
                {
                    float value = float.Parse(token);
                    return new ConstantToken(value);
                }
                else
                {
                    int value = int.Parse(token);
                    CheckIntegerOverflow(value);

                    return new ConstantToken(value);
                }
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
                CheckCharacter(token);

                return new ConstantToken(token[0]);
            }
            else
            {
                char previousCharacter = currentCharacter;
                currentCharacter = io.ReadNextCharacter();

                if (previousCharacter == '<' || previousCharacter == '>' || previousCharacter == ':' || previousCharacter == '(' || previousCharacter == '*' || previousCharacter == '.')
                {
                    string token = (previousCharacter + currentCharacter).ToString();

                    if (OperationMatcher.IsOperation(token))
                    {
                        currentCharacter = io.ReadNextCharacter();
                        return new OperationToken(OperationMatcher.GetOperation(token));
                    }
                }

                return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
            }
        }

        private void CheckForbiddenSymbol(string token)
        {
            if (token.IndexOf('!') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '!'");
            else if (token.IndexOf('@') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '@'");
            else if (token.IndexOf('#') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '#'");
            else if (token.IndexOf('$') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '$'");
            else if (token.IndexOf('%') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '%'");
            else if (token.IndexOf('&') != -1)
                io.Errors.Add("Ошибка. В строке запрещёный символ '&'");
        }

        private void CheckIntegerOverflow(int value)
        {
            if (value > 32768)
                io.Errors.Add("Ошибка. Значение целочисленной константы превышает предел");
        }

        private void CheckCharacter(string token)
        {
            if (token.Length != 1)
                io.Errors.Add("Ошибка. Символьная константа должна состоять из 1 символа");
        }
    }
}
