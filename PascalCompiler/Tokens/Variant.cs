
namespace PascalCompiler
{
    public enum VariantType
    {
        Integer,
        Real,
        String
    }

    public abstract class Variant
    {
        public VariantType Type { get; protected set; }
    }

    public class IntegerVariant : Variant
    {
        public int Value { get; }

        public IntegerVariant(int value)
        {
            Value = value;
            Type = VariantType.Integer;
        }

        public override string ToString() => $"{Type} | {Value}";
    }

    public class RealVariant : Variant
    {
        public double Value { get; }

        public RealVariant(double value)
        {
            Value = value;
            Type = VariantType.Real;
        }

        public override string ToString() => $"{Type} | {Value}";
    }

    public class StringVariant : Variant
    {
        public string Value { get; }

        public StringVariant(string value)
        {
            Value = value;
            Type = VariantType.String;
        }

        public override string ToString() => $"{Type} | {Value}";
    }
}