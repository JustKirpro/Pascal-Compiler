using System.Collections.Generic;

namespace PascalCompiler
{
    public static class Types
    {
        private static readonly Dictionary<string, Type> typesTable = new()
        {
            ["INTEGER"] = new IntegerType(),
            ["REAL"] = new RealType(),
            ["STRING"] = new StringType(),
            ["BOOLEAN"] = new BooleanType(),
            ["UNKNOWN"] = new UnknownType()
        };

        public static Type GetType(string type) => typesTable[type.ToUpper()];

        public static bool AreTypesDerived(Type leftPartType, Type rightPartType) => leftPartType.IsDerivedTo(rightPartType) || rightPartType.IsDerivedTo(leftPartType);

        public static Type DeriveTypes(Type leftPartType, Type rightPartType)
        {
            if (leftPartType is IntegerType && rightPartType is RealType || leftPartType is RealType && rightPartType is IntegerType)
                return typesTable["REAL"];

            return leftPartType;
        }
    }
}