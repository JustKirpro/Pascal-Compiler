
namespace PascalCompiler
{
    public enum ValueType
    {
        Integer,
        Real,
        String,
        Boolean
    };

    public abstract class Type
    {
        public ValueType ValueType { get; protected set; }

        public abstract bool IsDerivedTo(Type type);
    }

    public class IntegerType : Type
    {
        public IntegerType() => ValueType = ValueType.Integer;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Integer => true,
            ValueType.Real => true,
            _ => false
        };
    }

    public class RealType : Type
    {
        public RealType() => ValueType = ValueType.Real;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Real => true,
            _ => false
        };
    }

    public class StringType : Type
    {
        public StringType() => ValueType = ValueType.String;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.String => true,
            _ => false
        };
    }

    public class BooleanType : Type
    {
        public BooleanType() => ValueType = ValueType.Boolean;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Boolean => true,
            _ => false
        };
    }
}