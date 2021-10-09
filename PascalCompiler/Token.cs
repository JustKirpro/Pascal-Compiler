
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
        public string Operation { get; }

        public OperationToken(string operation)
        {
            Operation = operation;
            Type = TokenType.Operation;
        }

        public override string ToString() => $"{Type} | {Operation}";
    }

    public class ConstantToken : Token
    {
        public Variant Variant { get; }

        public ConstantToken(string constant)
        {
            if (int.TryParse(constant, out int intValue))
                Variant = new IntegerVariant(intValue);
            else if (float.TryParse(constant, out float floatValue))
                Variant = new FloatVariant(floatValue);
            else
                Variant = new StringVariant(constant);

            Type = TokenType.Constant;
        }

        public override string ToString() => $"{Type} | {Variant}";
    }
}