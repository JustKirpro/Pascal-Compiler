using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        public IOModule Io { get; private set; }
        private char currentCharacter;
        
        public LexicalAnalyzer(string inputPath, string outputPath)
        {
            Io = new(inputPath, outputPath);
            currentCharacter = ' ';
        }

        public Token GetNextToken()
        {
            if (!Io.HasUnreadCharacter())
                return null;

            while (currentCharacter == ' ' || currentCharacter == '{')
            {
                currentCharacter = Io.ReadNextCharacter();
                if (!Io.HasUnreadCharacter())
                    return null;

                if (currentCharacter == '{')
                {
                    while (currentCharacter != '}')
                        currentCharacter = Io.ReadNextCharacter();
                    currentCharacter = Io.ReadNextCharacter();
                }
            }

            StringBuilder stringBuilder = new();

            if (char.IsLetter(currentCharacter) || currentCharacter == '_')
            {
                while (currentCharacter != ' ' && !OperationMatcher.IsOperation(currentCharacter.ToString()))
                {
                    CheckForbiddenSymbol();
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = Io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();             
                return OperationMatcher.IsOperation(token) ? new OperationToken(OperationMatcher.GetOperation(token)) : new IdentifierToken(token);
            }
            else if (char.IsDigit(currentCharacter))
            {
                while (currentCharacter != ' ' && (!OperationMatcher.IsOperation(currentCharacter.ToString()) || currentCharacter == '.'))
                {
                    CheckForbiddenSymbol();
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = Io.ReadNextCharacter();
                }

                string token = stringBuilder.ToString();

                if (token.Contains('.'))
                {
                    double value = double.Parse(token);
                    return new ConstantToken(value);
                }
                else
                {
                    long value = long.Parse(token);

                    if (value > int.MaxValue)
                        Io.Errors.Add("Ошибка. Значение целочисленной константы превышает предел");

                    return new ConstantToken(value);
                }
            }
            else if (currentCharacter == '\'')
            {
                currentCharacter = Io.ReadNextCharacter();

                while (currentCharacter != '\'')
                {
                    stringBuilder.Append(currentCharacter);
                    currentCharacter = Io.ReadNextCharacter();
                }

                currentCharacter = Io.ReadNextCharacter();
                return new ConstantToken(stringBuilder.ToString());
            }
            else
            {
                char previousCharacter = currentCharacter;
                currentCharacter = Io.ReadNextCharacter();
                CheckForbiddenSymbol();

                if (previousCharacter == '<' || previousCharacter == '>' || previousCharacter == ':' || previousCharacter == '(' || previousCharacter == '*' || previousCharacter == '.')
                {
                    stringBuilder.Append(previousCharacter);
                    stringBuilder.Append(currentCharacter);
                    string token = stringBuilder.ToString();

                    if (OperationMatcher.IsOperation(token))
                    {
                        currentCharacter = Io.ReadNextCharacter();
                        return new OperationToken(OperationMatcher.GetOperation(token));
                    }
                }

                if (OperationMatcher.IsOperation(previousCharacter.ToString()))
                    return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
                else
                    return null;
            }
        }

        private void CheckForbiddenSymbol()
        {
            if (currentCharacter == '!' || currentCharacter == '%' || currentCharacter == '?' || currentCharacter == '&')
                Io.Errors.Add($"Ошибка. В строке запрещёный символ '{currentCharacter}'");
        }
    }
}
