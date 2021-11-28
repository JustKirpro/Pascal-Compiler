using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        private const int MaxIntValue = 32767;
        private const int MaxIdentifierLength = 127;
        public IOModule Io { get; private set; }
        private char currentCharacter;
        
        public LexicalAnalyzer(string inputPath, string outputPath)
        {
            Io = new(inputPath, outputPath);
            currentCharacter = ' ';
        }
      
        public Token GetNextToken()
        {
            while (currentCharacter == ' ' || currentCharacter == '\t')
                currentCharacter = Io.ReadNextCharacter();

            if (currentCharacter == '\0')
            {
                return null;
            }
            else if (currentCharacter == '\n')
            {
                currentCharacter = Io.ReadNextCharacter();
                return GetNextToken();
            }
            else if (currentCharacter == '\'')
            {
                return ScanString();
            }
            else if (char.IsDigit(currentCharacter))
            {
                return ScanNumber();
            }
            else if (char.IsLetter(currentCharacter))
            {
                return ScanKeywordOrIdentifier();
            }
            else if (currentCharacter == ')' || currentCharacter == ';' || currentCharacter == '=' || currentCharacter == ',' || currentCharacter == '^'
                                             || currentCharacter == ']' || currentCharacter == '+' || currentCharacter == '-' || currentCharacter == '/')
            {
                return ScanSingleCharacterOperator();
            }
            else if (currentCharacter == '(' || currentCharacter == '*' || currentCharacter == '{' || currentCharacter == '}' || currentCharacter == '<'
                                             || currentCharacter == '>' || currentCharacter == ':' || currentCharacter == '.') 
            {
                return ScanMultipleCharacterOperator();
            }
            else
            {
                AddError(7);
                currentCharacter = Io.ReadNextCharacter();
                return GetNextToken();
            }
        }

        private Token ScanString()
        {
            StringBuilder stringBuilder = new();
            currentCharacter = Io.ReadNextCharacter();

            while (currentCharacter != '\'' && currentCharacter != '\0')
            {
                stringBuilder.Append(currentCharacter);
                currentCharacter = Io.ReadNextCharacter();
            }

            string token = stringBuilder.ToString();

            if (currentCharacter == '\0' || token.Contains('\n'))
                AddError(3);

            if (currentCharacter == '\'')
                currentCharacter = Io.ReadNextCharacter();

            return new ConstantToken(token);
        }

        private Token ScanNumber()
        {
            StringBuilder stringBuilder = new();

            while (char.IsDigit(currentCharacter) || currentCharacter == '.')
            {
                stringBuilder.Append(currentCharacter);
                currentCharacter = Io.ReadNextCharacter();
            }

            string token = stringBuilder.ToString();

            if (token.Contains('.'))
            {
                if (double.TryParse(token, out double number) && !token.EndsWith('.'))
                {
                    return new ConstantToken(number);
                }
                else
                {
                    AddError(4);
                    return new ConstantToken(0.0);
                }
            }
            else
            {
                if (int.TryParse(token, out int number) && number <= MaxIntValue)
                {
                    return new ConstantToken(number);
                }
                else
                {
                    AddError(5);
                    return new ConstantToken(0);
                }
            }
        }

        private Token ScanKeywordOrIdentifier()
        {
            StringBuilder stringBuilder = new();

            while (char.IsLetter(currentCharacter) || char.IsDigit(currentCharacter) || currentCharacter == '_')
            {
                stringBuilder.Append(currentCharacter);
                currentCharacter = Io.ReadNextCharacter();
            }

            string token = stringBuilder.ToString();

            if (OperationMatcher.IsOperation(token))
            {
                return new OperationToken(OperationMatcher.GetOperation(token));
            }
            else
            {
                if (token.Length > MaxIdentifierLength)
                    AddError(6);

                return new IdentifierToken(token);
            }
        }

        private Token ScanSingleCharacterOperator()
        {
            char previousCharacter = currentCharacter;
            currentCharacter = Io.ReadNextCharacter();
            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
        }

        private Token ScanMultipleCharacterOperator()
        {
            if (currentCharacter == '(')
                return ScanLeftParenthesis();
            else if (currentCharacter == '*')
                return ScanAsterisk();
            else if (currentCharacter == '{')
                return ScanLeftBrace();
            else if (currentCharacter == '}')
                return ScanRightBrace();

            char previousCharacter = currentCharacter;
            currentCharacter = Io.ReadNextCharacter();

            string token = previousCharacter.ToString() + currentCharacter;

            if (OperationMatcher.IsOperation(token))
            {
                currentCharacter = Io.ReadNextCharacter();
                return new OperationToken(OperationMatcher.GetOperation(token));
            }

            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
        }

        private Token ScanLeftParenthesis()
        {
            currentCharacter = Io.ReadNextCharacter();

            if (currentCharacter == '*')
            {
                char previousCharacter = currentCharacter;
                currentCharacter = Io.ReadNextCharacter();

                while (previousCharacter != '*' || currentCharacter != ')' && currentCharacter != '\0')
                {
                    previousCharacter = currentCharacter;
                    currentCharacter = Io.ReadNextCharacter();
                }

                if (previousCharacter == '*' && currentCharacter == ')')
                {
                    return GetNextToken();
                }
                else
                {
                    AddError(1);
                    return null;
                }
            }
            else
            {
                return new OperationToken(Operation.LeftParenthesis);
            }
        }

        private Token ScanAsterisk()
        {
            currentCharacter = Io.ReadNextCharacter();

            if (currentCharacter == ')')
            {
                currentCharacter = Io.ReadNextCharacter();
                AddError(2);
                return GetNextToken();
            }
            else
            {
                return new OperationToken(Operation.Asterisk);
            }
        }

        private Token ScanLeftBrace()
        {
            currentCharacter = Io.ReadNextCharacter();

            while (currentCharacter != '}' && currentCharacter != '\0' && currentCharacter != '\n')
                currentCharacter = Io.ReadNextCharacter();

            if (currentCharacter == '}')
            {
                currentCharacter = Io.ReadNextCharacter();
                return GetNextToken();
            }

            AddError(1);

            if (currentCharacter == '\0')
            {
                return null;
            }
            else
            {
                return GetNextToken();
            }
        }

        private Token ScanRightBrace()
        {
            currentCharacter = Io.ReadNextCharacter();
            AddError(2);
            return GetNextToken();
        }

        private void AddError(int errorCode)
        {
            Io.Errors.Add(new Error(errorCode));
        }
    }
}