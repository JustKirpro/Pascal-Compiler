
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
    }

    public class IdentifierToken : Token
    {
        public string Identifier { get; }

        public IdentifierToken(string identifier)
        {
            Identifier = identifier;
            Type = TokenType.Identifier;
        }

        public override string ToString() => $"{Type} | {Identifier}";
    }

    public class OperationToken : Token
    {
        public Operation Operation { get; }

        public OperationToken(Operation operation)
        {
            Operation = operation;
            Type = TokenType.Operation;
        }

        public override string ToString() => $"{Type} | {Operation}";
    }

    public class ConstantToken : Token
    {
        public Variant Variant { get; }

        public ConstantToken(int constant)
        {
            Variant = new IntegerVariant(constant);
            Type = TokenType.Constant;
        }

        public ConstantToken(float constant)
        {
            Variant = new FloatVariant(constant);
            Type = TokenType.Constant;
        }

        public ConstantToken(char constant)
        {
            Variant = new StringVariant(constant);
            Type = TokenType.Constant;
        }

        public override string ToString() => $"{Type} | {Variant}";
    }
}