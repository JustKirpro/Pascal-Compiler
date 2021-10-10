
namespace PascalCompiler
{
    public enum VariantType
    {
        Integer,
        Float,
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

    public class FloatVariant : Variant
    {
        public float Value { get; }

        public FloatVariant(float value)
        {
            Value = value;
            Type = VariantType.Float;
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