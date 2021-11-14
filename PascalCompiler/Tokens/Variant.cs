﻿
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
        public long Value { get; }

        public IntegerVariant(long value)
        {
            Value = value;
            Type = VariantType.Integer;
        }

        public override string ToString() => $"{Type} | {Value}";
    }

    public class FloatVariant : Variant
    {
        public double Value { get; }

        public FloatVariant(double value)
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