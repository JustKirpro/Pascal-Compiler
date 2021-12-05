
namespace PascalCompiler
{
    public enum TokenType
    {
        Identifier,
        Operation,
        Constant
    }

    public abstract class Token
    {
        public TokenType Type { get; protected set; }
        public int StartPosition;
    }

    public class IdentifierToken : Token
    {
        public string Identifier { get; }

        public IdentifierToken(string identifier, int startPosition)
        {
            Identifier = identifier;
            Type = TokenType.Identifier;
            StartPosition = startPosition;
        }

        public override string ToString() => $"{Type} | {Identifier}";
    }

    public class OperationToken : Token
    {
        public Operation Operation { get; }

        public OperationToken(Operation operation, int startPosition)
        {
            Operation = operation;
            Type = TokenType.Operation;
            StartPosition = startPosition;
        }

        public override string ToString() => $"{Type} | {Operation}";
    }

    public class ConstantToken : Token
    {
        public Variant Variant { get; }

        public ConstantToken(int constant, int startPosition)
        {
            Variant = new IntegerVariant(constant);
            Type = TokenType.Constant;
            StartPosition = startPosition;
        }

        public ConstantToken(double constant, int startPosition)
        {
            Variant = new RealVariant(constant);
            Type = TokenType.Constant;
            StartPosition = startPosition;
        }

        public ConstantToken(string constant, int startPosition)
        {
            Variant = new StringVariant(constant);
            Type = TokenType.Constant;
            StartPosition = startPosition;
        }

        public override string ToString() => $"{Type} | {Variant}";
    }
}