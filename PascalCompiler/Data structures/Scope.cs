using System.Collections.Generic;

namespace PascalCompiler
{
    public class Scope
    {
        private readonly Dictionary<string, Type> typesTable = new()
        {
            ["INTEGER"] = new IntegerType(),
            ["REAL"] = new RealType(),
            ["STRING"] = new StringType(),
            ["UNKNOWN"] = new UnknownType()
        };
        private readonly Dictionary<IdentifierToken, Type> variablesTable = new();

        public Scope() { }

        public bool IsTypeAvailable(IdentifierToken type) => typesTable.ContainsKey(type.Identifier.ToUpper()) && !IsVariableDescribed(type);

        public bool IsVariableDescribed(IdentifierToken newVariable)
        {
            foreach (IdentifierToken variable in variablesTable.Keys)
                if (newVariable.Identifier == variable.Identifier)
                    return true;

            return false;
        }

        public void AddVariable(IdentifierToken variable, IdentifierToken type)
        {
            if (IsVariableDescribed(variable))
            {
                IdentifierToken describedVariable = GetVariable(variable);

                if (typesTable[type.Identifier.ToUpper()] != variablesTable[describedVariable])
                {
                    variablesTable.Remove(describedVariable);
                    variablesTable.Add(variable, typesTable["UNKNOWN"]);
                }
            }
            else
            {
                variablesTable.Add(variable, typesTable[type.Identifier.ToUpper()]);
            }
        }

        public void AddVariable(IdentifierToken variable)
        {
            if (IsVariableDescribed(variable))
            {
                IdentifierToken describedVariable = GetVariable(variable);
                variablesTable.Remove(describedVariable);
            }

            variablesTable.Add(variable, typesTable["UNKNOWN"]);
        }

        public Type GetVariableType(IdentifierToken variable) => variablesTable[GetVariable(variable)];

        private IdentifierToken GetVariable(IdentifierToken newVariable)
        {
            foreach (IdentifierToken variable in variablesTable.Keys)
                if (newVariable.Identifier == variable.Identifier)
                    return variable;

            return null;
        }
    }
}