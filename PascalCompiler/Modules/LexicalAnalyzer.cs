using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        private const int MaxIntValue = 32767;
        private const int MaxIdentifierLength = 127;
        private readonly IOModule IOModule;
        private char currentCharacter;
        
        public LexicalAnalyzer(string inputPath, string outputPath)
        {
            IOModule = new(inputPath, outputPath);
            currentCharacter = ' ';
        }
      
        public Token GetNextToken()
        {
            while (currentCharacter == ' ' || currentCharacter == '\t' || currentCharacter == '\n')
                currentCharacter = IOModule.ReadNextCharacter();

            if (currentCharacter == '\0')
            {
                return null;
            }
            else if (currentCharacter == '\'')
            {
                return ScanString();
            }
            else if (char.IsDigit(currentCharacter))
            {
                return ScanNumber();
            }
            else if (char.IsLetter(currentCharacter) || currentCharacter == '_')
            {
                return ScanKeywordOrIdentifier();
            }
            else if (currentCharacter == ')' || currentCharacter == ';' || currentCharacter == '=' || currentCharacter == ',' || currentCharacter == '^'
                                             || currentCharacter == ']' || currentCharacter == '+' || currentCharacter == '-' || currentCharacter == '/'
                                             || currentCharacter == '{' || currentCharacter == '}')
            {
                return ScanSingleCharacterOperator();
            }
            else if (currentCharacter == '(' || currentCharacter == '*' || currentCharacter == '<' || currentCharacter == '>' || currentCharacter == ':'
                                             || currentCharacter == '.') 
            {
                return ScanMultipleCharacterOperator();
            }
            else
            {
                AddError(7);
                currentCharacter = IOModule.ReadNextCharacter();
                return GetNextToken();
            }
        }

        private Token ScanString()
        {
            StringBuilder stringBuilder = new();
            currentCharacter = IOModule.ReadNextCharacter();

            while (currentCharacter != '\'' && currentCharacter != '\0')
            {
                stringBuilder.Append(currentCharacter);
                currentCharacter = IOModule.ReadNextCharacter();
            }

            string token = stringBuilder.ToString();

            if (currentCharacter == '\0' || token.Contains('\n'))
                AddError(3);

            if (currentCharacter == '\'')
                currentCharacter = IOModule.ReadNextCharacter();

            return new ConstantToken(token);
        }

        private Token ScanNumber()
        {
            StringBuilder stringBuilder = new();

            while (char.IsDigit(currentCharacter) || currentCharacter == '.')
            {
                stringBuilder.Append(currentCharacter);
                currentCharacter = IOModule.ReadNextCharacter();
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
                currentCharacter = IOModule.ReadNextCharacter();
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
            if (currentCharacter == '{') 
                return ScanLeftBrace();
            else if (currentCharacter == '}')
                return ScanRightBrace();

            char previousCharacter = currentCharacter;
            currentCharacter = IOModule.ReadNextCharacter();
            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
        }

        private Token ScanMultipleCharacterOperator()
        {
            if (currentCharacter == '(')
                return ScanLeftParenthesis();
            else if (currentCharacter == '*')
                return ScanAsterisk();

            char previousCharacter = currentCharacter;
            currentCharacter = IOModule.ReadNextCharacter();

            string token = previousCharacter.ToString() + currentCharacter;

            if (OperationMatcher.IsOperation(token))
            {
                currentCharacter = IOModule.ReadNextCharacter();
                return new OperationToken(OperationMatcher.GetOperation(token));
            }

            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()));
        }

        private Token ScanLeftParenthesis()
        {
            currentCharacter = IOModule.ReadNextCharacter();

            if (currentCharacter == '*')
            {
                char previousCharacter = currentCharacter;
                currentCharacter = IOModule.ReadNextCharacter();

                while (previousCharacter != '*' || currentCharacter != ')' && currentCharacter != '\0')
                {
                    previousCharacter = currentCharacter;
                    currentCharacter = IOModule.ReadNextCharacter();
                }

                if (previousCharacter == '*' && currentCharacter == ')')
                {
                    currentCharacter = IOModule.ReadNextCharacter();
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
            currentCharacter = IOModule.ReadNextCharacter();

            if (currentCharacter == ')')
            {
                currentCharacter = IOModule.ReadNextCharacter();
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
            currentCharacter = IOModule.ReadNextCharacter();

            while (currentCharacter != '}' && currentCharacter != '\0' && currentCharacter != '\n')
                currentCharacter = IOModule.ReadNextCharacter();

            if (currentCharacter == '}')
            {
                currentCharacter = IOModule.ReadNextCharacter();
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
            currentCharacter = IOModule.ReadNextCharacter();
            AddError(2);
            return GetNextToken();
        }

        public void AddError(int errorCode)
        {
            IOModule.AddError(errorCode);
        }
    }
}