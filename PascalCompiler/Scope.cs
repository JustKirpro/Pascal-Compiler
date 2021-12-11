using System.Collections.Generic;

namespace PascalCompiler
{
    public class Scope
    {
        private readonly Dictionary<string, Type> typesTable = new()
        {
            { "INTEGER", new IntegerType() },
            { "REAL", new RealType() },
            { "STRING", new StringType() }
        };
        private readonly Dictionary<IdentifierToken, Type> variablesTable = new();

        public Scope() { }

        public bool IsTypeAvailable(IdentifierToken type) => typesTable.ContainsKey(type.Identifier.ToUpper());

        public bool IsVariableDescribed(IdentifierToken variable) => variablesTable.ContainsKey(variable);

        public void AddVariable(IdentifierToken variable, IdentifierToken type) => variablesTable.Add(variable, typesTable[type.Identifier.ToUpper()]);
    }
}
