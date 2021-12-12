using System.Text;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        private const int MaxIntValue = 32767;
        private const int MaxIdentifierLength = 127;
        private readonly IOModule IOModule;
        private char currentCharacter;
        private int startPosition;
        
        public LexicalAnalyzer(string inputPath, string outputPath)
        {
            IOModule = new(inputPath, outputPath);
            currentCharacter = ' ';
        }

        public void AddError(int code) => IOModule.AddError(code, startPosition);

        public void AddError(int code, int position) => IOModule.AddError(code, position);

        public Token GetNextToken()
        {
            while (currentCharacter == ' ' || currentCharacter == '\t' || currentCharacter == '\n')
                currentCharacter = IOModule.ReadNextCharacter();

            startPosition = IOModule.CharacterNumber;

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
            bool wasErrorFound = false;
            StringBuilder stringBuilder = new();
            currentCharacter = IOModule.ReadNextCharacter();

            while (currentCharacter != '\'' && currentCharacter != '\0')
            {
                if (currentCharacter == '\n' && !wasErrorFound)
                {
                    AddError(3);
                    wasErrorFound = true;
                }

                stringBuilder.Append(currentCharacter);
                currentCharacter = IOModule.ReadNextCharacter();
            }

            string token = stringBuilder.ToString();

            if (currentCharacter == '\'')
                currentCharacter = IOModule.ReadNextCharacter();

            return new ConstantToken(token, startPosition);
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
                    return new ConstantToken(number, startPosition);
                }
                else
                {
                    AddError(4);
                    return new ConstantToken(0.0, startPosition);
                }
            }
            else
            {
                if (int.TryParse(token, out int number) && number <= MaxIntValue)
                {
                    return new ConstantToken(number, startPosition);
                }
                else
                {
                    AddError(5);
                    return new ConstantToken(0, startPosition);
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
                return new OperationToken(OperationMatcher.GetOperation(token), startPosition);
            }
            else
            {
                if (token.Length > MaxIdentifierLength)
                    AddError(6);

                return new IdentifierToken(token, startPosition);
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
            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()), startPosition);
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
                return new OperationToken(OperationMatcher.GetOperation(token), startPosition);
            }

            return new OperationToken(OperationMatcher.GetOperation(previousCharacter.ToString()), startPosition);
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
                return new OperationToken(Operation.LeftParenthesis, startPosition);
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
                return new OperationToken(Operation.Asterisk, startPosition);
            }
        }

        private Token ScanLeftBrace()
        {
            currentCharacter = IOModule.ReadNextCharacter();

            while (currentCharacter != '{' && currentCharacter != '\0' && currentCharacter != '\n')
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
    }
}