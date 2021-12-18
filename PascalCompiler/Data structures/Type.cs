using System.Collections.Generic;

namespace PascalCompiler
{
    public enum ValueType
    {
        Integer,
        Real,
        String,
        Boolean,
        Unknown
    };

    public abstract class Type
    {
        public ValueType ValueType { get; protected set; }

        public abstract bool IsDerivedTo(Type type);

        public abstract bool IsOperationSupported(Operation operation);
    }

    public class IntegerType : Type
    {
        private readonly static List<Operation> supportedOperations = new() { Operation.Plus, Operation.Minus, Operation.Or, Operation.Asterisk, Operation.Mod, Operation.Div, Operation.And };

        public IntegerType() => ValueType = ValueType.Integer;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Integer => true,
            ValueType.Real => true,
            ValueType.Unknown => true,
            _ => false
        };

        public override bool IsOperationSupported(Operation operation) => supportedOperations.Contains(operation);
    }

    public class RealType : Type
    {
        protected readonly static List<Operation> supportedOperations = new() { Operation.Plus, Operation.Minus, Operation.Asterisk, Operation.Slash };

        public RealType() => ValueType = ValueType.Real;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Real => true,
            ValueType.Unknown => true,
            _ => false
        };

        public override bool IsOperationSupported(Operation operation) => supportedOperations.Contains(operation);
    }

    public class StringType : Type
    {
        protected readonly static List<Operation> supportedOperations = new() { Operation.Plus };

        public StringType() => ValueType = ValueType.String;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.String => true,
            ValueType.Unknown => true,
            _ => false
        };

        public override bool IsOperationSupported(Operation operation) => supportedOperations.Contains(operation);
    }

    public class BooleanType : Type
    {
        protected readonly static List<Operation> supportedOperations = new() { Operation.Or, Operation.And };

        public BooleanType() => ValueType = ValueType.Boolean;

        public override bool IsDerivedTo(Type type) => type.ValueType switch
        {
            ValueType.Boolean => true,
            ValueType.Unknown => true,
            _ => false
        };

        public override bool IsOperationSupported(Operation operation) => supportedOperations.Contains(operation);
    }

    public class UnknownType : Type
    {
        public UnknownType() => ValueType = ValueType.Unknown;

        public override bool IsDerivedTo(Type type) => true;

        public override bool IsOperationSupported(Operation operation) => true;
    }
}